using System.Linq.Expressions;
using NPoco;
using Umbraco.Cms.Core.Models;
using Umbraco.UIBuilder;
using Umbraco.UIBuilder.Persistence;

namespace Umbraco.Cms.v13_x.UIBuilder;

public class IntSubThingRepository : Repository<SubThing<int>, int>
{
    private List<SubThing<int>> _list = new List<SubThing<int>>()
    {
        {
            new SubThing<int>()
            {
                Id = 99,
                ParentThingId = 1, Name = "Subthing 1"
            }
        }
    };

    public IntSubThingRepository(RepositoryContext context) : base(context)
    {
    }

    protected override int GetIdImpl(SubThing<int> entity) => entity.Id;

    protected override SubThing<int> GetImpl(int id)
    {
        var entity = _list.Find(x => x.Id == id);
        if (entity == null)
            return null;
        var model = entity.Copy();
        return model;
    }

    protected override IEnumerable<TJunctionEntity> GetRelationsByParentIdImpl<TJunctionEntity>(int parentId, string relationAlias) => throw new NotImplementedException();

    protected override SubThing<int> SaveImpl(SubThing<int> model)
    {
        var entity = _list.Find(x => x.Id == model.Id);
        if (entity == null)
            return null;

        entity.Name = model.Name;
        entity.ParentThingId = model.ParentThingId;
        entity.TestProperty = model.TestProperty;

        return model;
    }

    protected override TJunctionEntity SaveRelationImpl<TJunctionEntity>(TJunctionEntity entity) => throw new NotImplementedException();

    protected override void DeleteImpl(int id) => throw new NotImplementedException();

    protected override IEnumerable<SubThing<int>> GetAllImpl(Expression<Func<SubThing<int>, bool>>? whereClause = null,
        Expression<Func<SubThing<int>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? (thing => true);
        var filtered = _list.Where(where);

        return filtered;
    }

    protected override PagedResult<SubThing<int>> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<SubThing<int>, bool>>? whereClause = null, Expression<Func<SubThing<int>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? ((thing) => true);
        var filtered = _list.Where(where);

        return new PagedResult<SubThing<int>>(filtered.Count(), 1, 1) { Items = filtered };
    }

    protected override long GetCountImpl(Expression<Func<SubThing<int>, bool>> whereClause)
    {
        var x = whereClause?.Compile() ?? (thing => true);
        return _list.Count(x);
    }
}
