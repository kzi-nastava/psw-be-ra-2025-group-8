using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.Events;
using Xunit;
using System;
using System.Linq;

namespace Explorer.Blog.Tests.Unit.Domain
{
    public class BlogPostCommentTests
    {
        private BlogPost CreatePublishedBlog()
        {
            // simulate creation and publishing of blog
            var blog = new BlogPost(
                authorId: 1,
                title: "Test Blog",
                description: "Test opis",
                images: null
            );
            blog.Publish();
            return blog;
        }
        private Comment CreateComment(long personId, DateTime creationTime)
        {
            return new Comment(
                personId: personId,
                creationTime: creationTime,
                text: "Originalni tekst komentara"
            );
        }

        private BlogPost CreatePublishedBlogWithComment(long commentAuthorId, DateTime creationTime)
        {
            var blog = CreatePublishedBlog();

            var comment = CreateComment(commentAuthorId, creationTime);

            blog.Comments.Add(comment);

            return blog;
        }

        [Fact]
        public void Can_add_comment_to_published_blog()
        {
            // Arrange
            var blog = CreatePublishedBlog();
            long testUserId = 2;
            string commentText = "Ovo je test komentar.";

            // Act
            var newComment = blog.AddComment(testUserId, commentText);

            // Assert
            Assert.Single(blog.Comments);
            Assert.Equal(testUserId, newComment.PersonId);
            Assert.Equal(commentText, newComment.Text);
            Assert.True(DateTime.UtcNow.Subtract(newComment.CreationTime).TotalSeconds < 5);
            Assert.Null(newComment.LastEditTime);
            Assert.Single(blog.DomainEvents.Where(e => e is CommentCreatedEvent));

            var createdEvent = (CommentCreatedEvent)blog.DomainEvents.First(e => e is CommentCreatedEvent);
            Assert.Equal(blog.Id, createdEvent.BlogId);

            blog.ClearDomainEvents();
        }

        [Theory]
        [InlineData(BlogStatus.Draft)]
        [InlineData(BlogStatus.Archived)]
        public void Cannot_add_comment_to_non_published_blog(BlogStatus status)
        {
            // Arrange
            var blog = new BlogPost(1, "Test Blog", "Test opis", null);

            if (status == BlogStatus.Archived)
            {
                blog.Publish();
                blog.Archive();
            }

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                blog.AddComment(2, "Neuspešan komentar");
            });

            Assert.Empty(blog.Comments);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Cannot_create_comment_with_empty_text(string emptyText)
        {
            // Arrange
            var blog = CreatePublishedBlog();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                blog.AddComment(2, emptyText);
            });

            Assert.Empty(blog.Comments);
        }
    
        [Fact]
        public void Can_update_comment_within_15_minutes_and_if_author()
        {
            // Arrange
            long authorId = 5;
            var creationTime = DateTime.UtcNow.AddMinutes(-5);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;
            var newText = "Ažurirani tekst komentara.";

            // Act
            blog.UpdateComment(commentId, authorId, newText);

            // Assert
            var updatedComment = blog.Comments.First();
            Assert.Equal(newText, updatedComment.Text);
            Assert.NotNull(updatedComment.LastEditTime);
            Assert.True(updatedComment.LastEditTime > updatedComment.CreationTime);
        }

        [Fact]
        public void Cannot_update_comment_if_not_author()
        {
            // Arrange
            long authorId = 5;
            long nonAuthorId = 6;
            var creationTime = DateTime.UtcNow.AddMinutes(-5);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                blog.UpdateComment(commentId, nonAuthorId, "Neuspešna izmena");
            });

            // Assert
            Assert.Equal("Originalni tekst komentara", blog.Comments.First().Text);
        }

        [Fact]
        public void Cannot_update_comment_after_15_minutes()
        {
            // Arrange
            long authorId = 5;
            var creationTime = DateTime.UtcNow.AddMinutes(-20);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                blog.UpdateComment(commentId, authorId, "Prekasna izmena");
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Cannot_update_comment_with_empty_text(string emptyText)
        {
            // Arrange
            long authorId = 5;
            var creationTime = DateTime.UtcNow.AddMinutes(-5);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                blog.UpdateComment(commentId, authorId, emptyText);
            });
        }

        [Fact]
        public void Can_delete_comment_within_15_minutes_and_if_author()
        {
            // Arrange
            long authorId = 5;
            var creationTime = DateTime.UtcNow.AddMinutes(-5);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;

            // Act
            blog.DeleteComment(commentId, authorId);

            // Assert
            Assert.Empty(blog.Comments);
        }

        [Fact]
        public void Cannot_delete_comment_if_not_author()
        {
            // Arrange
            long authorId = 5;
            long nonAuthorId = 6;
            var creationTime = DateTime.UtcNow.AddMinutes(-5);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                blog.DeleteComment(commentId, nonAuthorId);
            });

            // Assert
            Assert.Single(blog.Comments);
        }

        [Fact]
        public void Cannot_delete_comment_after_15_minutes()
        {
            // Arrange
            long authorId = 5;
            var creationTime = DateTime.UtcNow.AddMinutes(-20);
            var blog = CreatePublishedBlogWithComment(authorId, creationTime);
            var commentId = blog.Comments.First().Id;

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                blog.DeleteComment(commentId, authorId);
            });

            // Assert
            Assert.Single(blog.Comments);
        }
    }
}