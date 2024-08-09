using System.Linq.Expressions;
using NPoco;
using Umbraco.Cms.Core.Models;
using Umbraco.UIBuilder;
using Umbraco.UIBuilder.Persistence;

namespace Umbraco.Cms.v13_x.UIBuilder;

public class GuidSubThingRepository : Repository<SubThing<Guid>, Guid>
{
    private List<SubThing<Guid>> _list = new List<SubThing<Guid>>()
    {
        {
            new SubThing<Guid>()
            {
                Id = new Guid("95f41a15-dfd6-4ae3-8309-208a349924d1"),
                ParentThingId = new Guid("721bdde3-badb-4602-bd70-7725db9824fe"), Name = "Subthing 1"
            }
        }
    };

    public GuidSubThingRepository(RepositoryContext context) : base(context)
    {
    }

    protected override Guid GetIdImpl(SubThing<Guid> entity) => entity.Id;

    protected override SubThing<Guid> GetImpl(Guid id)
    {
        var entity = _list.Find(x => x.Id == id);
        if (entity == null)
            return null;
        var model = entity.Copy();
        return model;
    }

    protected override IEnumerable<TJunctionEntity> GetRelationsByParentIdImpl<TJunctionEntity>(Guid parentId, string relationAlias) => throw new NotImplementedException();

    protected override SubThing<Guid> SaveImpl(SubThing<Guid> model)
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

    protected override void DeleteImpl(Guid id) => throw new NotImplementedException();

    protected override IEnumerable<SubThing<Guid>> GetAllImpl(Expression<Func<SubThing<Guid>, bool>>? whereClause = null,
        Expression<Func<SubThing<Guid>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? (thing => true);
        var filtered = _list.Where(where);

        return filtered;
    }

    protected override PagedResult<SubThing<Guid>> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<SubThing<Guid>, bool>>? whereClause = null, Expression<Func<SubThing<Guid>, object>>? orderBy = null,
        SortDirection orderByDirection = SortDirection.Ascending)
    {
        var where = whereClause?.Compile() ?? ((thing) => true);
        var filtered = _list.Where(where);

        return new PagedResult<SubThing<Guid>>(filtered.Count(), 1, 1) { Items = filtered };
    }

    protected override long GetCountImpl(Expression<Func<SubThing<Guid>, bool>> whereClause)
    {
        var x = whereClause?.Compile() ?? (thing => true);
        return _list.Count(x);
    }
}
