using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace Umbraco.Cms.v13_x.UIBuilder;

public class TestDataPickerSource : IDataPickerSource
{
    private IContentmentEntityContext _entityContext;
    public TestDataPickerSource(IContentmentEntityContext entityContext)
    {
        this._entityContext = entityContext;
    }
    public string? Name => "Test Data picker source";
    public string? Description=> default;
    public string? Icon => default;
    public Dictionary<string, object>? DefaultValues => default;
    public IEnumerable<ConfigurationField> Fields => default;
    public string? Group => default;
    public OverlaySize OverlaySize => default;

    public async Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
    {
        var id = this._entityContext.GetCurrentEntityId(out bool isParent);
        return new DataListItem[] { new DataListItem() { Name = $"Test Item [{id}][IsParent? {isParent}]", Value = id } };
    }

    public async Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1,
        int pageSize = 12, string query = "")
    {
        var id = this._entityContext.GetCurrentEntityId(out bool isParent);
        var items = new DataListItem[] { new DataListItem() { Name = $"Test Item [{id}][IsParent? {isParent}]", Value = id } };
        return new PagedResult<DataListItem>(items.Length, 1, 10) { Items = items };
    }
}
