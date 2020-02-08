window.dialogs = {

    HttpJob: {
        Register: function (args) {
            return $.dialog({ action: actions.HttpJob.Register(args), title: "HTTP JOB", width: 800, height: 600 });
        }
    },

    SqlCommandJob: {
        Register: function (args) {
            return $.dialog({ action: actions.SqlCommandJob.Register(args), title: "SQL COMMAND JOB", width: 800, height: 600 });
        }
    },

    SqlMailReportJob: {
        Register: function (args) {
            return $.dialog({ action: actions.SqlMailReportJob.Register(args), title: "SQL MAIl REPORT JOB", width: 800, height: 600 });
        }
    },
};