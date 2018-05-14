# social-network

This is a social network application built with ASP.NET Core.

Besides just ASP.NET Core, I have also used Entity Framework Core 2, Identity, and plain JavaScript. 
The architecture is somewhat similar to Clean/Onion because the project is split into many small projects.

Project is using some practices from functional programming such as pure methods and method chaining.
To accomplish better method chaining I have used some of my generic extension methods like Let and Also (like in Kotlin), which enables me to chain almost any method calls.

Application is using the in memory database, so it should be easier for you to check it out.

And about the functionalities, it is a social network that enables registered users to create, edit, delete, and rate posts.
Users can edit and delete only their own posts, and like/dislike posts from other users.
