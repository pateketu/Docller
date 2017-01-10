docller.companyNames = null;

docller.Company = function () {
    var self = this;
    self.companyName = "";
    self.users = "";

};
var CompaniesModel = function (additonalData) {
    var self = this;
    var index = 0;
    self.additionalData = additonalData;
    self.companies = ko.observableArray([
        new docller.Company("")]);

    self.addCompany = function () {
        self.companies.push(new docller.Company());
        index++;
        $("#companyName_" + index).typeahead(options);
    };

    self.removeCompany = function (company) {
        self.companies.remove(company);
        index = index - 1;
    };

    self.save = function (component, evt) {
        $("#errors").empty();
        var hasErrors = false;
        var numOfEmpty = 0;
        //do some basic validation
        for (var i = 0; i < self.companies().length; i++) {

            if (self.companies()[i].companyName.trim().length == 0
                && self.companies()[i].users.trim().length == 0) {
                numOfEmpty++;
            }
            if (self.companies()[i].companyName.trim().length != 0
                && self.companies()[i].users.trim().length == 0) {
                hasErrors = true;
                $("#errors").append("<div class='label label-important'>Please provide emails for " + self.companies()[i].companyName.trim() + "</div><br/>");
            }
            if (self.companies()[i].companyName.trim().length == 0
                && self.companies()[i].users.trim().length != 0) {
                hasErrors = true;
                $("#errors").append("<div class='label label-important'>Please provide company names</div><br/>");
            }

        }
        if (numOfEmpty == self.companies().length) {
            hasErrors = true;
            $("#errors").append("<div class='label label-important'>Please provide Companies and E-mails</div>");
        }
        
        if (!hasErrors) {
            var data = JSON.stringify(ko.toJS(self.companies), null, 2);
            var postData = "data=" + data;
            var postUrl = $(evt.target).data("url");

            if (self.additionalData) {
                postData = postData + "&" + self.additionalData();
            }

            $.ajax({
                type: "POST",
                url: postUrl,
                data: postData,
                success: function(result) {
                    if (!result.success) {
                        for (var j = 0; j < result.errors.length; j++) {
                            $("#errors").append("<div class='label label-important'>" + result.errors[j] + "</div><br/>");
                        }
                    } else {
                        $("#inviteUserContainer").html("<div class='alert alert-success'>Successfully send invitations.</div>");
                    }
                },
                error: function() {
                    alert("Errors occured in sending invitations, Please try again later");
                },
                dataType: "json"
            });
        }
    };

};

//ko.applyBindings(new CompaniesModel(), document.getElementById("inviteUserContainer"));


var options = {
    source: function (query, process) {
        if (docller.companyNames == null) {
            docller.companyNames = [];
            for (var i = 0; i < docller.subscribers.length; i++) {
                docller.companyNames.push(docller.subscribers[i].companyName);
            }
        }
        return docller.companyNames;
    },
    updater: function (item) {
        var users = null;
        for (var i = 0; i < docller.subscribers.length; i++) {
            if (docller.subscribers[i].companyName == item) {
                users = docller.subscribers[i].users;
                break;
            }
        }
        if (users != null) {
            var emails = "";
            for (var j = 0; j < users.length; j++) {
                emails += users[j].email + "\n";
            }
            var target = this.$element.attr('data-typeahead-target');
            $("#" + target).val(emails).trigger("change");
        }

        return item;
    }
};

