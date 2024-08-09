namespace Umbraco.Cms.v13_x.UIBuilder;

public class Thing<TKey> where TKey : struct
{

    public TKey Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string[] TestProperty { get; set; }
}

public class SubThing<TKey> where TKey : struct
{
    public TKey Id { get; set; }
    public TKey ParentThingId { get; set; }
    public string Name { get; set; }
    public string[] TestProperty { get; set; }
}
