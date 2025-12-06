using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class BlogImage : Entity
{
    public string Url { get; private set; }
    public int Order { get; private set; }

    protected BlogImage() { }

    public BlogImage(string url, int order)
    {
        Url = url;
        Order = order;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Url)) throw new ArgumentException("Invalid Url");
        if (Order < 0) throw new ArgumentException("Invalid Order");
    }
}
