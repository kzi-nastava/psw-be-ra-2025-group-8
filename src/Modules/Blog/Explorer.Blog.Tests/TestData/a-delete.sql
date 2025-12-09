DELETE FROM blog."BlogImages"
WHERE "BlogPostId" < 0;

DELETE FROM blog."BlogPosts"
WHERE "Id" < 0;
