window.views = (function (views) {

    views.SqlCommandJob = {

        Register: function () {

            var $my = $.myContext("#SqlCommandJobRegister");

            var $form = $my('form');

            $form.validate();


            $my('#save').click(function () {

                if ($form.valid()) {

                    actions.SqlCommandJob.Save($form.toJSON())
                        .done(function (result) {
                            if (result.Id) {
                                $.mensagemSalvo();
                                $form.closeDialog();
                            }
                        });
                }
            });


            $my('#cancel').click(function () { $form.closeDialog(); });
        }
    };

    return views;

})(window.views || {});