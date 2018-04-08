﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DevelopmentUtilities;
using SocialNetwork.Interfaces.DAL;
using SocialNetwork.Interfaces.Services;
using SocialNetwork.Models;
using SocialNetwork.Web.ServiceInterfaces;
using SocialNetwork.Web.ViewModels;
using SocialNetwork.Web.Constants;
using Utilities;

namespace SocialNetwork.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IViewRendererService _renderer;
        private readonly IHub _hub;
        private readonly IPostsRepository _posts;
        private readonly IDatabaseOperations _dbOps;
        private readonly IRepository<User> _users;

        public HomeController(IViewRendererService renderer, IHub hub, IRepository<User> users,
                              IPostsRepository posts, IDatabaseOperations dbOps)
        {
            _renderer = renderer;
            _hub = hub;
            _posts = posts;
            _dbOps = dbOps;
            _users = users;
        }

        public async Task<IActionResult> Index()
        {
            var a = await _posts.GetMany(count: 5, order: PostsOrder.CreatedAt_Descending, propsToInclude: "Author");

            User currentUser = await _users.GetOne(it => it.UserName == User.Identity.Name);

            var vm = new HomeViewModel
            {
                Posts = a.AsReadOnly(),
                Title = "Social Network",
                ActivePage = Page.Home,
                Username = currentUser.UserName
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(string postText)
        {
            User author = await _users.GetOne(it => it.UserName == User.Identity.Name);
            Post post = Generator.GeneratePost(text: postText, createdAt: DateTime.Today, author: author, likesCount: 0, dislikesCount: 0);
            Post storedPost = _posts.Insert(post);

            Task saveChangesTask = _dbOps.SaveChangesAsync();
            string postHtml = await _renderer.RenderPartialView("_Post", storedPost);
            await saveChangesTask;
            await _hub.Emit(PostsEvents.PostPublished, postHtml);

            return Ok("");
        }

        [HttpPost]
        public async Task<IActionResult> LoadPosts(int count, int skip, string[] propsToInclude)
        {
            var posts = await _posts.GetMany(
                order: PostsOrder.CreatedAt_Descending,
                propsToInclude: propsToInclude,
                count: count,
                skip: skip
            );
            var renderingTasks = posts.Select(it => _renderer.RenderPartialView("_Post", it));
            var rendered = await Task.WhenAll(renderingTasks);
            return Ok(rendered);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost(long id, string text = null, string heading = null,
                                                    bool addLike = false, bool addDislike = false)
        {
            Post post = await _posts.GetOne(it => it.Id == id, "Author");

            post.Text = text ?? post.Text;
            post.Heading = heading ?? post.Heading;
            if (addLike)
                post.LikesCount++;
            if (addDislike)
                post.DislikesCount++;

            _posts.Update(post);
    
            Task saveChangesTask = _dbOps.SaveChangesAsync();
            string postHtml = await _renderer.RenderPartialView("_Post", post);
            await saveChangesTask;
            await _hub.Emit(PostsEvents.PostChanged, postHtml);

            return Ok("");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(long postId)
        {
            await _posts.Delete(it => it.Id == postId);

            await _dbOps.SaveChangesAsync();
            await _hub.Emit(PostsEvents.PostDeleted, postId);

            return Ok("");
        }
    }
}
