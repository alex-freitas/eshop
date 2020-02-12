window.views = (function (views) {

    views.Job = {

        Index: function () {

            var $my = $.myContext("#JobIndex");


            var $form = $my("form");


            var updateSearch = function () {

                var $def = $.Deferred();

                $my("input[name=SortBy]").val("Name ASC");

                if ($form.valid()) {

                    actions.Job.SearchCount($form.toJSON())
                        .done(function (rows) {

                            $my(".paginator").paginator({
                                currentPage: 1,
                                totalRows: parseInt(rows)
                            });

                            updateTable().done(function () { $def.resolve() });
                        }
                        );
                }

                return $def;
            };


            var updateTable = function () {
                return $my("tbody:first").ajaxLoad(actions.Job.SearchResult($form.toJSON()))
                    .done(function () {

                        $my("i[name=delete]").click(function () {

                            if (!$.confirmarExclusao())
                                return;

                            actions.Job.Delete({ id: $(this).parents("tr:first").data("id") })
                                .done(updateSearch);
                        });

                        $my("i[name=edit]").click(function () {

                            var $tr = $(this).parents("tr:first");
                            var jobType = $tr.data("jobType");

                            dialogs[jobType].Register({ id: $tr.data("id") })
                                .done(updateSearch);
                        });
                    });
            };


            $my(".paginator").paginator({
                startRowInput: $my("[name=StartRow]"),
                maxRowsInput: $my("[name=MaxRows]"),
                pageChanged: updateTable
            });


            $my("table:first").ordenator({
                sortByHidden: $my("input[name=SortBy]"),
                updateCallback: updateTable
            });


            $my("#search").click(function () {
                updateSearch().done(function () { $.mensagemConsultaAtualizada() });
            });


            $my("#addJob").click(function () { $my("#jobsToAdd").slideToggle(); });

            $my("#addSqlCommandJob").click(function () {
                dialogs.SqlCommandJob.Register().done(updateSearch);
            });

            $my("#addSqlMailReportJob").click(function () {
                dialogs.SqlMailReportJob.Register().done(updateSearch);
            });

            $my("#addHttpJob").click(function () {
                dialogs.HttpJob.Register().done(updateSearch);
            });

            $my("#toggleFilters").click(function () { $my("#filters").slideToggle(); });


            updateSearch();
        }
    };

    return views;

})(window.views || {});