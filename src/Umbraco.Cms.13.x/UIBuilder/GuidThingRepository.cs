using System.Linq.Expressions;
using NPoco;
using Umbraco.Cms.Core.Models;
using Umbraco.UIBuilder;
using Umbraco.UIBuilder.Persistence;

namespace Umbraco.Cms.v13_x.UIBuilder;

public class GuidThingRepository : Repository<Thing<Guid>, Guid>
{
    private List<Thing<Guid>> _list = new List<Thing<Guid>>()
    {
        {
            new Thing<Guid>()
            {
                Id = new Guid("721bdde3-badb-4602-bd70-7725db9824fe"), Name = "Item 1", Description = "Test thing 1"
            }
        },
        {
            new Thing<Guid>()
            {
                Id = new Guid("be545b0c-d773-4071-89d4-7ad740ff88f7"), Name = "Item 2", Description = "Test thing 2"
            }
        }
    };

    public GuidThingRepository(RepositoryContext context) : base(context)
    {
    }

    protected override Guid GetIdImpl(Thing<Guid> entity) => entity.Id;

    protected override Thing<Guid> GetImpl(Guid id)
    {
        var entity = _list.Find(x => x.Id == id);
        if (entity == null)
            return null;
        return entity.Copy();

    }

    protected override IEnumerable<TJunctionEntity> GetRelationsByParentIdImpl<TJunctionEntity>(Guid parentId, string relationAlias) => throw new NotImplementedException();

    protected override Thing<Guid> SaveImpl(Thing<Guid> model)
    {
        var entity = _list.Find(x => x.Id == model.Id);
        if (entity == null)
            return null;

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.TestProperty = model.TestProperty;
        return model;
    }

    protected override TJunctionEntity SaveRelationImpl<TJunctionEntity>(TJunctionEntity entity) => throw new NotImplementedException();

    protected override void DeleteImpl(Guid id) => throw new NotImplementedException();

    protected override IEnumerable<Thing<Guid>> GetAllImpl(Expression<Func<Thing<Guid>, bool>>? whereClause = null,
        Expression<Func<Thing<Guid>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? (thing => true);
        var filtered = _list.Where(where);

        return filtered;
    }

    protected override PagedResult<Thing<Guid>> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<Thing<Guid>, bool>>? whereClause = null, Expression<Func<Thing<Guid>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? ((thing) => true);
        var filtered = _list.Where(where);

        return new PagedResult<Thing<Guid>>(filtered.Count(), 1, 1) { Items = filtered };
    }

    protected override long GetCountImpl(Expression<Func<Thing<Guid>, bool>> whereClause)
    {
        var x = whereClause?.Compile() ?? (thing => true);
        return _list.Count(x);
    }
}
