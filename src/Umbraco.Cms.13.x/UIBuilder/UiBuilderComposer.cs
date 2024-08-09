using Umbraco.Cms.Core.Composing;
using Umbraco.UIBuilder.Extensions;

namespace Umbraco.Cms.v13_x.UIBuilder;

public class UiBuilderComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddUIBuilder(cfg =>
        {
            cfg.AddSectionAfter("media", "Repositories", sectionConfig => sectionConfig
                .Tree(treeConfig => treeConfig

                    .AddCollection<Thing<Guid>>(thing => thing.Id,
                        "Guid Thing",
                        "Guid Things",
                        "Test Guid Things",
                        "icon-thumbnail-list",
                        "icon-thumbail-list",
                        collectionConfig => collectionConfig
                            .SetNameProperty(x => x.Name)
                            .SetRepositoryType<GuidThingRepository>()
                            .Editor(editorConfig => editorConfig
                                .AddTab("General", tabConfig => tabConfig
                                    .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                        .AddField(x => x.Description))
                                    .AddFieldset("Test Properties", fieldsetConfig => fieldsetConfig
                                        .AddField(x => x.TestProperty)
                                        .SetDataType("[Contentment] Data Picker - Test Data Source"))
                                )
                            )
                            .AddChildCollection<SubThing<Guid>>(x => x.Id,
                                x => x.ParentThingId,
                                "Guid Subthing",
                                "Guid Subthings",
                                "Guid Sub-things",
                                "icon-sitemap",
                                "icon-sitemap",
                                subCollection => subCollection
                                    .SetNameProperty(y => y.Name)
                                    .SetRepositoryType<GuidSubThingRepository>()
                                    .Editor(editorConfig => editorConfig
                                        .AddTab("General", tabConfig => tabConfig
                                            .AddFieldset("Test Properties", fieldsetConfig => fieldsetConfig
                                                .AddField(x => x.TestProperty)
                                                .SetDataType("[Contentment] Data Picker - Test Data Source"))
                                        )
                                    )
                            )
                    )
                    .AddCollection<Thing<int>>(thing => thing.Id,
                        "Int Thing",
                        "Int Things",
                        "Test Int Things",
                        "icon-thumbnail-list",
                        "icon-thumbail-list",
                        collectionConfig => collectionConfig
                            .SetNameProperty(x => x.Name)
                            .SetRepositoryType<IntThingRepository>()
                            .Editor(editorConfig => editorConfig
                                .AddTab("General", tabConfig => tabConfig
                                    .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                        .AddField(x => x.Description))
                                    .AddFieldset("Test Properties", fieldsetConfig => fieldsetConfig
                                        .AddField(x => x.TestProperty)
                                        .SetDataType("[Contentment] Data Picker - Test Data Source"))
                                )
                            )
                            .AddChildCollection<SubThing<int>>(x => x.Id,
                                x => x.ParentThingId,
                                "Int Subthing",
                                "Int Subthings",
                                "Int Sub-things",
                                "icon-sitemap",
                                "icon-sitemap",
                                subCollection => subCollection
                                    .SetNameProperty(y => y.Name)
                                    .SetRepositoryType<IntSubThingRepository>()
                                    .Editor(editorConfig => editorConfig
                                        .AddTab("General", tabConfig => tabConfig
                                            .AddFieldset("Test Properties", fieldsetConfig => fieldsetConfig
                                                .AddField(x => x.TestProperty)
                                                .SetDataType("[Contentment] Data Picker - Test Data Source"))
                                        )
                                    )
                                ))
                ));

        });




    }
}
