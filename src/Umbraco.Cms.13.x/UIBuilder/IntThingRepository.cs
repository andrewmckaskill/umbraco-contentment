using System.Linq.Expressions;
using NPoco;
using Umbraco.Cms.Core.Models;
using Umbraco.UIBuilder;
using Umbraco.UIBuilder.Persistence;

namespace Umbraco.Cms.v13_x.UIBuilder;

public class IntThingRepository : Repository<Thing<int>, int>
{
    private List<Thing<int>> _list = new List<Thing<int>>()
    {
        {
            new Thing<int>()
            {
                Id = 1, Name = "Item 1", Description = "Test thing 1"
            }
        },
        {
            new Thing<int>()
            {
                Id = 2, Name = "Item 2", Description = "Test thing 2"
            }
        }
    };

    public IntThingRepository(RepositoryContext context) : base(context)
    {
    }

    protected override int GetIdImpl(Thing<int> entity) => entity.Id;

    protected override Thing<int> GetImpl(int id)
    {
        var entity = _list.Find(x => x.Id == id);
        if (entity == null)
            return null;
        return entity.Copy();

    }

    protected override IEnumerable<TJunctionEntity> GetRelationsByParentIdImpl<TJunctionEntity>(int parentId, string relationAlias) => throw new NotImplementedException();

    protected override Thing<int> SaveImpl(Thing<int> model)
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

    protected override void DeleteImpl(int id) => throw new NotImplementedException();

    protected override IEnumerable<Thing<int>> GetAllImpl(Expression<Func<Thing<int>, bool>>? whereClause = null,
        Expression<Func<Thing<int>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? (thing => true);
        var filtered = _list.Where(where);

        return filtered;
    }

    protected override PagedResult<Thing<int>> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<Thing<int>, bool>>? whereClause = null, Expression<Func<Thing<int>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? ((thing) => true);
        var filtered = _list.Where(where);

        return new PagedResult<Thing<int>>(filtered.Count(), 1, 1) { Items = filtered };
    }

    protected override long GetCountImpl(Expression<Func<Thing<int>, bool>> whereClause)
    {
        var x = whereClause?.Compile() ?? (thing => true);
        return _list.Count(x);
    }
}
