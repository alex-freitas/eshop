;window.actions=(function(n){
n.Job={};
n.Job.Index=function(p){return $.ajax({url:'/',type:'GET',data:p})};
n.Job.Error=function(p){return $.ajax({url:'/Job/Error',type:'GET',data:p})};
n.Job.SearchResult=function(p){return $.ajax({url:'/Job/SearchResult',type:'GET',data:p})};
n.Job.SearchCount=function(p){return $.ajax({url:'/Job/SearchCount',type:'GET',data:p})};
n.Job.Register=function(p){return $.ajax({url:'/Job/Register',type:'GET',data:p})};
n.Job.Delete=function(p){return $.ajax({url:'/Job/Delete',type:'POST',data:p})};
return n;})(window.actions||{});
;window.actions=(function(n){
n.SqlCommandJob={};
n.SqlCommandJob.Register=function(p){return $.ajax({url:'/SqlCommandJob/Register',type:'GET',data:p})};
n.SqlCommandJob.Save=function(p){return $.ajax({url:'/SqlCommandJob/Save',type:'POST',data:p})};
return n;})(window.actions||{});
