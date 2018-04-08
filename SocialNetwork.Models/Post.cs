﻿using System;
using System.Collections.Generic;
using System.Data;

namespace SocialNetwork.Models
{
    public class Post : IEntity<long>
    {
        public long Id { get; set; }

        public string Heading { get; set; }

        public string Text { get; set; }

        public string AuthorId { get; set; }

        public int LikesCount { get; set; }

        public int DislikesCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public User Author { get; set; }

        public ICollection<PostRating> Ratings { get; set; } = new HashSet<PostRating>();
    }
}
