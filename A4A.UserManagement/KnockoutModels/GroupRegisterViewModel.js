/// <reference path="../Scripts/knockout-3.1.0.js" />
/// <reference path="../Scripts/jquery-1.10.2.js" />

var SendEmailAdmin = new Map();
var A4AModelEmailAdmin = new Array();

var CompanyNamePrimary = new Map();
var A4AModelCompanyNamePrimary = new Array();
var GetCompanyNamePrimary = new Map();

var CompanyNameAlternate = new Map();
var A4AModelCompanyNameAlternate = new Array();
var GetCompanyNameAlternate = new Map();

var ChkStaff = new Map();
var A4AModelChkStaff = new Array();

var ChkGroupUser = new Map();
var A4AModelChkGroupUser = new Array();

var chkInformationalUser = new Map();
var A4AModelInformationalUser = new Array();

var chkContactUser = new Map();
var A4AModelContactUser = new Array();

var CheckStatusStaff = ko.observable(false);
var CheckStatusGroupUser = ko.observable(false);

var chkTaskGroupUser = new Map();
var A4AModelTaskGroupUser = new Array();

function clearalltimeouts() {
    var highestTimeoutId = setTimeout(";");
    for (var i = 0; i < highestTimeoutId; i++) {
        clearTimeout(i);
        alert(i);
    }
}

var shouldShowDelete = ko.observable(true);
var isAdmin = ko.observable(true);
var groupRegisterViewModel;
/////////////////////////////////////////////////////
// TAB 1 - Group Properties Registration views view model
/////////////////////////////////////////////////////
function Group(HiddenGroupId, GroupName, AppliesToSiteId, GroupTypeId, LyrisListName, LyrisShortDescription, GroupSiteUrl, IsCommittee,
    IsChildGroup, ParentGroupId, CouncilParentId, IsSecurityGroup, Liaison1, Mission, LyrisSendId, BounceReports, GAB,
    requestedTeamMembers, requestedSelectedMembers) {
    var self = this;
    // observable are update elements upon changes, also update on element data changes [two way binding] 
    self.HiddenGroupId = ko.observable(HiddenGroupId);
    self.GroupName = ko.observable(GroupName);//.extend({ required: true });//.extend({ required: "Please enter a Group Name" });
    self.AppliesToSiteId = ko.observable(AppliesToSiteId);
    self.GroupTypeId = ko.observable(GroupTypeId);//.extend({ required: true });
    self.GroupTypeName = ko.observable();
    self.LyrisListName = ko.observable(LyrisListName);//.extend({ required: true });   //.extend({ required: { message:"Please enter a Lyris List Name. <br/>" } });
    self.LyrisShortDescription = ko.observable(LyrisShortDescription);//.extend({ required: true }); //.extend({ required: "Please enter a Lyrs Short Description" });
    self.GroupSiteUrl = ko.observable(GroupSiteUrl);
    self.ParentGroupId = ko.observable(ParentGroupId);
    self.Liaison1 = ko.observable(Liaison1);//.extend({ required: true });
    self.Liaison2 = ko.observable();
    self.ShowL2 = ko.observable(false);
    self.Mission = ko.observable(Mission);
    self.LyrisSendId = ko.observable();
    self.DepartmentId = ko.observable();
    self.rdoBounceReports = ko.observable("Off");
    self.InfoMessage = ko.observable();
    self.gId = ko.observable();
    self.edit = ko.observable(false);

    ///^[a-z0-9]+$/i
    var patterns = {
        groupemail: /^[a-z0-9_-]+$/i
    };
    self.step1Validation = ko.validatedObservable([
        self.GroupName.extend({ required: { message: ' Group Name is required.' } }),
        self.LyrisListName.extend({
            required: { message: ' Use only lower-case alphanumeric characters (a-z, 0-9), hyphens (-),or underscores ( _ ). Do not use spaces.​' },
            pattern: {
                message: ' Use only lower-case alphanumeric characters (a-z, 0-9).​.',
                params: patterns.groupemail
            }
        }),
        self.LyrisListName.extend({ required: { message: ' List Name is required.' } }),
        self.Liaison1.extend({ required: { message: ' Please choose a Liaison.' } }),
        self.DepartmentId.extend({ required: { message: ' Please choose a Department.' } }),
        self.LyrisSendId.extend({ required: { message: 'Please choose a Email Send Permissions.' } })
    ]);
    self.BounceReports = ko.computed(function () {
        if (self.rdoBounceReports() === "On")
            return true;
        else
            return false;
    });
    self.rdoGAB = ko.observable("Off");
    self.GAB = ko.computed(function () {
        if (self.rdoGAB() === "On")
            return true;
        else
            return false;
    });

    self.requestedTeamMembers = ko.observableArray(requestedTeamMembers);
    self.requestedSelectedMembers = ko.observableArray(requestedSelectedMembers);
    /***************************************/
    //Set show errors boolean
    /***************************************/
    self.ShowErr = ko.observable(false);
    self.GroupName.subscribe(function () {
        self.removeErrors();
        $('#hdnGroupName').val(self.GroupName());
        self.LyrisShortDescription(self.GroupName());
    });
    self.LyrisListName.subscribe(function () {
        self.removeErrors();
    });
    self.removeErrors = function () {
        self.ShowErr(false);
    };
    /********************************************/
    /* Databind Lyris Send */
    /********************************************/
    self.LyrisSends = ko.observableArray();
    self.getData1 = function () {
        $.ajax({
            url: '/api/LyrisSend',
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                for (key in result) {
                    var item = {
                        name: key,         // Push the key on the array
                        value: result[key] // Push the key's value on the array
                    };
                    self.LyrisSends.push(item);
                }
            },
            error: function (exception) {
            }
        });
    };
    /********************************************/
    /* Databind Department Send */
    /********************************************/
    self.Dept = ko.observableArray();
    self.getDepartments = function () {
        $.ajax({
            url: '/api/GetDept',
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var result = JSON.parse(data);
                $.each(result, function (key, value) {
                    var item = {
                        name: value.DepartmentDetail, // Push the key on the array
                        value: value.DepartmentId // Push the key's value on the array
                    };
                    self.Dept.push(item);
                });
            },
            error: function (exception) {
            }
        });
    };
    /********************************************/
    /* Databind Get all Group Send */
    /********************************************/
    self.ParentGroups = ko.observableArray();
    self.getData2 = function () {
        $.ajax({
            url: '/api/getGroupNamesWithId',
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                for (key in result) {
                    var item = {
                        name: key,         // Push the key on the array
                        value: result[key] // Push the key's value on the array
                    };
                    self.ParentGroups.push(item);
                }
            },
            error: function (exception) {
            }
        });
    };
    /********************************************/
    /* Databind Get GroupType */
    /********************************************/
    self.GroupTypes = ko.observableArray();
    self.getDataGroupTypes = function () {
        $.ajax({
            url: '/api/getGroupType',
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                for (key in result) {
                    var item = {
                        value: key,// Push the key on the array
                        name: result[key] // Push the value of the key on the array
                    };
                    self.GroupTypes.push(item);
                }
            },
            error: function (exception) {
            }
        });
    };

    /********************************************/
    /* Databind Get Liaisons */
    /********************************************/
    self.Liaisons = ko.observableArray();
    self.getDataLiaisons = function () {
        $.ajax({
            url: '/api/GetA4AStaffwTitle',
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                for (key in result) {
                    if (!(result === null || result === "undefined")) {
                        var res = result[key].split(":");
                        var item = {
                            value: res[1],// Push the key on the array
                            name: res[0]// Push the value of the key on the array                    
                        };
                        self.Liaisons.push(item);
                    }
                }
            },
            error: function (exception) {
            }
        });
    };
    self.AppliesToSiteIdValues = ko.observableArray([
        { id: "2", name: "Members" },
        { id: "1", name: "Fuels" },
        { id: "3", name: "Both" }
    ]);
    self.ShowErr = ko.observable(false);

    self.addGroup = function () {
        //Reset Info Message
        self.InfoMessage("");
        //get the Mission editor data
        var editor = CKEDITOR.instances.inpMission;
        groupRegisterViewModel.addGroupViewModel.Mission(editor.getData());
        var result = ko.validation.group(groupRegisterViewModel.addGroupViewModel, { deep: true });
        $(".overlay").show();
        //Don't need validation anymore -  || editor.length > 0 || editor.getData() == ''
        if (!self.step1Validation.isValid()) {
            self.ShowErr(true);
            self.step1Validation.errors.showAllMessages();
            //Checking if mission is filled - Commented checking Mission for now. Because it will cause a problem at launch. 9/25/2018
            //console.log(CKEDITOR.instances.inpMission.getData());
            $(".overlay").hide();
            return false;
        }
        else {
            $('.overlay').show();
            self.ShowErr(false);
            var dataObject = ko.toJSON(this);
            $.ajax({
                url: '/api/group?isEdit=' + self.edit(),
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                },
                error: function (exception) {
                    $(".overlay").hide();
                    $("#CrtGrpMessage").css("color", "red");
                    self.InfoMessage(exception.responseText);
                }
            }).done(function (data) {
                $(".overlay").hide();
                $("#CrtGrpMessage").css("color", "green");
                self.edit(true);
                self.InfoMessage("Group has been saved successfully");
                self.HiddenGroupId(data.GroupId);
                $('#hdnGroupId').val(data.GroupId);
                $('#hdnGroupName').val(self.GroupName());
                $("#Headtitle").html("Create New Group");
                $(".NoEdit").prop("disabled", true).css("background-color", "lightgrey");
                groupRegisterViewModel.addA4AStaffViewModel.gId(data.GroupId);
                groupRegisterViewModel.UMSubscribeModel.gId(data.GroupId);
                groupRegisterViewModel.addTaskForceWorkGroup.gId(data.GroupId);
                councilCommitteeViewModel.gId = data.GroupId;
                groupRegisterViewModel.addA4AStaffViewModel.hasBounceReports(data.BounceReports);
                enableForm();
                $('#hdnSaveGroupValue').val("1");
                var EmailSendId = $('input[name="LyrisSends"]:checked').val();
                if (EmailSendId != "1") {
                    $("input[name='EmailAdmin']").prop("disabled", true);
                    $('#DivSaveInformationalRoles').hide();
                    $('#DivSaveContactsRoles').hide();
                    $('#DivSaveCompanyRoles').hide();
                    $('#DivSaveCommitteeRoles').hide();
                    $('#DivSaveTaskGroupRoles').hide();
                }
                else {
                    $("input[name='EmailAdmin']").prop("disabled", false);
                    $('#DivSaveInformationalRoles').show();
                    $('#DivSaveContactsRoles').show();
                    $('#DivSaveCompanyRoles').show();
                    $('#DivSaveCommitteeRoles').show();
                    $('#DivSaveTaskGroupRoles').show();
                }
            });
        }
    };
    self.load = function (grp) {
        try {
            var ATAgroup = JSON.parse(grp.responseText);
            self.GroupName(ATAgroup.GroupName);
            self.AppliesToSiteId(ATAgroup.AppliesToSiteId);
            self.GroupTypeId(ATAgroup.GroupTypeId);
            self.LyrisListName(ATAgroup.LyrisListName);
            self.LyrisShortDescription(ATAgroup.LyrisShortDescription);
            self.GroupSiteUrl(ATAgroup.GroupSiteUrl);
            self.ParentGroupId(ATAgroup.ParentGroupId);
            self.Liaison1(ATAgroup.Liaison1UserId);
            self.Liaison2(ATAgroup.Liaison2UserId);
            if (ATAgroup.Liaison2UserId > 0) {
                self.ShowL2(true);
            }
            self.Mission(ATAgroup.Mission);
            self.LyrisSendId("" + ATAgroup.LyrisSendId);
            self.DepartmentId(ATAgroup.DepartmentId);
            $('#hdnchkRadioId').val(ATAgroup.LyrisSendId);
            if (ATAgroup.BounceReports)
                self.rdoBounceReports("On");
            else
                self.rdoBounceReports("Off");
            if (ATAgroup.GAB)
                self.rdoGAB("On");
            else
                self.rdoGAB("Off");

            self.edit(true);
            $(".NoEdit").prop("disabled", true).css("background-color", "lightgrey");
            //10/22/2018 - NA - wizywig has issues loading mission data so moved it to bottom.  moved to wizywig instantioed method in index. 
        }
        catch (err) {
            alert("Error loading group data." + err.message);
        }
    };
}
/////////////////////////////////////////////////////////////////////////////
/***********TAB 2 -- A4A Staff KO binding Object****************************/
///////////////////////////////////////////////////////////////////////////// 
var A4AStaffModel = function (A4AStaff) {
    var self = this;
    self.selected = ko.observable();
    self.A4AStaff = ko.observableArray(A4AStaff);
    self.A4AStaffSelected = ko.observable();
    self.gId = ko.observable();
    self.hasBounceReports = ko.observable(false);
    self.UMContacts = ko.observableArray();
    self.InfoMessage = ko.observable();
    self.ErrorMessage = ko.observable();
    self.GroupName = ko.observable(); //Assigned in the index.html
    self.isDistribution = ko.observable(false);
    self.edit = ko.observable(false);
    self.A4AStaffList = ko.observableArray();
    self.getA4AStaffList = function () {
        $.ajax({
            url: '/api/GetA4AStaffwTitle',
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                for (key in result) {
                    if (!(result === null || result === "undefined")) {
                        var res = result[key].split(":");

                        var item = {
                            value: res[1],// Push the key on the array
                            name: res[0]// Push the value of the key on the array                    
                        };
                        self.A4AStaffList.push(item);
                    }
                }
            },
            error: function (exception) { }
        });
    };
    self.setChosen = function () {
        ko.bindingHandlers.chosen =
        {
            init: function (element) {
                $(element).addClass('chzn-select');
                $(element).chosen();
            },
            update: function (element) {
                $(element).trigger('liszt:updated');
            }
        };
    };
    self.selected.subscribe(function (newValue) {
        if (!isNaN(self.gId())) {
            for (var key in newValue) {
                //Check if the user already exists
                var match = ko.utils.arrayFirst(self.A4AStaff(),
                    function (item) {
                        return (newValue[key].value == item.UserId);
                    });
                if (!match) {
                    self.A4AStaff.push({
                        GroupId: self.gId(),//264,
                        UserId: newValue[key].value,
                        Name: newValue[key].name,
                        ManageGroup: false,
                        EmailAdmin: false,
                        BounceReports: false,
                        StaffSubscribe: false,
                        Alerts: false,
                        hasBounceReports: self.hasBounceReports()
                    });

                    $('#z-tab2').find('a').show(); $('#z-tab2').find('input').prop('disabled', false);
                    if (self.gId() > 0 && self.gId() != null) {
                        if ($('#hdnchkRadioId').val() != "1") {
                            $("input[name='EmailAdmin']").prop("disabled", true);
                        }
                        else {
                            $("input[name='EmailAdmin']").prop("disabled", false);
                        }
                    }
                } else {
                    self.ErrorMessage("User" + newValue[key].name + " already exists!");
                    setTimeout(function () { self.ErrorMessage(""); }, 6000);
                }
            }
        }
        else {
            self.ErrorMessage("No Group ID exists!");
            setTimeout(function () { self.ErrorMessage(""); }, 6000);
        }

    });
    self.removeA4AStaff = function (staff) {
        $('.overlay').show();
        var gtypeid = $('#hdnGroupTypeId').val();

        //IMPORTANT NOTE- Changing to this so we remove all roles for the staff even if users deselect some checkboxes
        var url = '/api/deletea4ausergroups?GroupId=' + staff["GroupId"] + '&UserId=' + staff["UserId"] + '&IsA4AStaff=true';
        var dataObject = ko.utils.stringifyJson(staff);
        dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null 

        $.ajax({
            url: url,
            type: 'post', //Original commenetd 7/28/20;
            /// data: dataObject, Original commenetd 7/28/20
            contentType: 'application/json',
            success: function (data) {
                self.A4AStaff.remove(staff);
                getManagedGroupCount(self.A4AStaff()[0].GroupId);
                self.InfoMessage("A4A staff removed successfully!");
                setTimeout(function () { self.InfoMessage(""); }, 6000);
            },
            error: function (xhr, status, error) {
                $(".overlay").hide();
                self.ErrorMessage("Error:" + xhr.responseText);
                setTimeout(function () { self.ErrorMessage(""); }, 15000);

                return false;
            }
        });
        $(".overlay").hide();
    };

    self.removeUMContacts = function (contact) {
        self.A4AStaff.remove(contact);
    };

    self.save = function (form) {
        $('.overlay').show();
        var dataObject = ko.utils.stringifyJson(self.A4AStaff);
        var hasRole = true; //Setting the bool for checking that roles are selcted for all 

        for (var i = 0; i < self.A4AStaff().length; i++) {
            var GroupId = self.A4AStaff()[i].GroupId;
            var Name = self.A4AStaff()[i].Name;
            var ManageGroup = self.A4AStaff()[i].ManageGroup;
            var EmailAdmin = self.A4AStaff()[i].EmailAdmin;
            var BounceReports = self.A4AStaff()[i].BounceReports;
            var StaffSubscribe = self.A4AStaff()[i].StaffSubscribe;
            //Set bool to see if a role was selected
            if (ManageGroup || EmailAdmin || BounceReports || StaffSubscribe)
                hasRole = true;
            else
                hasRole = false;
            if (!hasRole) {
                self.ErrorMessage("All staff members in this list should have at least one role. Please choose a role for staff member - " + Name + " before saving!");
                setTimeout(function () {
                    self.ErrorMessage("");
                }, 10000);
                $(".overlay").hide(); //hide the overlay before stopping the execution
                return;
            }
        }
        $.ajax({
            url: '/api/A4AUserGroup/?IsStaff=true&IsGGA=false',
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                self.InfoMessage("A4A Staff changes saved successfully");
                getManagedGroupCount(self.A4AStaff()[0].GroupId);
                setTimeout(function () {
                    self.InfoMessage("");
                }, 6000);
            },
            error: function (xhr, status, error) {
                self.ErrorMessage("Error:" + xhr.responseText);
                $(".overlay").hide();
                setTimeout(function () { self.ErrorMessage(""); }, 10000);
                return false;
            }
        }).done(function (data) {
            $(".overlay").hide();
        });
    };

    self.load = function () {
        var query = "/api/geta4ausergroups/?GroupId=" + self.gId() + "&IsA4AStaff=" + true;
        $.ajax({
            url: query,
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                for (i = 0; i < result.length; i++)
                    self.A4AStaff.push({
                        GroupId: result[i].GroupId,
                        UserId: result[i].UserId,
                        Name: result[i].UserName,
                        ManageGroup: result[i].ManageGroup,
                        EmailAdmin: result[i].EmailAdmin,
                        BounceReports: result[i].BounceReports,
                        StaffSubscribe: result[i].StaffSubscribe,
                        Alerts: result[i].Alerts,
                        hasBounceReports: self.hasBounceReports()
                    });
            },
            error: function (exception) {
                self.InfoMessage('Exception in Get Liaisons:' + exception.responseText);
                setTimeout(function () { self.InfoMessage(""); }, 10000);
            }
        });
    }
};

/////////////////////////////////////////////////////////////////////
/*******TAB 5&6 and Part of Council Committee - A
Complete Subscribe/Informational tab KO binding Object********/
/////////////////////////////////////////////////////////////////////  
ko.bindingHandlers.ko_autocomplete = {
    init: function (element, params) {
        $(element).autocomplete(params());
    },
    update: function (element, params) {
        $(element).autocomplete("option", "source", params().source);
    }
};
var AutoCompleteExample;
(function (AutoCompleteExample) {
    var ViewModel = (function () {
        function ViewModel() {
            this.selectedValue = ko.observable();
            this.selectedValuesNW = ko.observableArray();
        }

        ViewModel.prototype.gId = ko.observable(0);
        ViewModel.prototype.errorMessage = ko.observable();
        ViewModel.prototype.InfoMessage = ko.observable();
        ViewModel.prototype.edit = ko.observable(false);
        ViewModel.prototype.clearSubscribertxt = function () {
            try {
                $(".acSubInfoUser").val("");
            }
            catch (exception) {
                alert(exception);
            }
        };

        ViewModel.prototype.SorI = 0;
        ViewModel.prototype.getLanguages = function (request, response) {
            $(".overlay").show();
            var gtypeid = $('#hdnGroupTypeId').val();
            var url = '';
            if ($('#hdnGroupTypeId').val())
                url = '/api/GetAllContacts/?grouptype=' + gtypeid;
            else
                url = '/api/GetAllContacts/';
            $.ajax({
                url: url,
                type: 'GET',
                cache: false,
                data: request,
                dataType: 'json',
                success: function (json) {
                    response($.map(json, function (key, value) {
                        return {
                            label: value, //exchanged by NA - changed the reuslt to sorted dictionary 3/26/2019
                            text: key,
                            value: ""
                        }
                    }));
                }
            });
            $(".overlay").hide();
        };
        ViewModel.prototype.selectedValues = ko.observableArray();
        /////////////////////////////////////////////////////////////////////
        /***  Subscribe/Informational -  Select User to Add     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectLanguage = function (event, ui) {
            try {
                $(".overlay").show();
                //Get Gid now 
                var gId = 0;
                gId = $('#hdnGroupId').val(); ///check for groupid value.TODO:cleaning up the code 
                if (!(gId > 0) && !(ViewModel.prototype.gId() > 0)) {
                    ViewModel.prototype.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
                    setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 6000);
                }
                else {
                    ViewModel.prototype.errorMessage("");
                }
                var role = '';
                var temp, temp1;
                //Get CompanyName out of the username
                var UsernCompname = ui.item.label;
                var startindex = UsernCompname.indexOf('(');
                var cname = UsernCompname.substring(startindex + 1).replace(')', ''); //Get the Company out of the Company Name string.
                var usrname = UsernCompname.replace(UsernCompname.substring(startindex), ''); //Get the companyname out of the username
                if (+$("#hdnSubscribeInformational").val() === 1) //ViewModel.prototype.SorI not working for somereason so changed to hidden field.
                {
                    role = "Participant";
                    temp = [{ "Name": UsernCompname, "UserId": ui.item.text, "Participant": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname, "CheckStatus": false }];
                    temp1 = { "Name": UsernCompname, "UserId": ui.item.text, "Participant": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname, "CheckStatus": false };
                }
                else
                    if (+$("#hdnSubscribeInformational").val() === 2) {
                        role = "Informational";
                        temp = [{ "Name": UsernCompname, "UserId": ui.item.text, "Informational": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname, "CheckStatus": false }];
                        temp1 = { "Name": UsernCompname, "UserId": ui.item.text, "Informational": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname, "CheckStatus": false };
                    }
                    else {
                        ViewModel.prototype.errorMessage("Subscription group type is not selected, Contact IT for help!");
                        setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 6000);
                        return;
                    }

                var dataObject = ko.utils.stringifyJson(temp);
                $.ajax({
                    url: '/api/UserGroup',
                    type: 'post',
                    data: dataObject,
                    async: false,
                    contentType: 'application/json',
                    success: function (data) {
                        ViewModel.prototype.selectedValues.push(temp1);
                        ViewModel.prototype.InfoMessage("User " + usrname + " has been successfully added!");
                        setTimeout(function () { ViewModel.prototype.InfoMessage(""); $('#z-tab4').find('a').show(); if ($('#hdnchkRadioId').val() != "1") { $("input[name='EmailAdmin']").prop("disabled", true); $('#DivSaveInformationalRoles').hide(); } else { $("input[name='EmailAdmin']").prop("disabled", false); $('#DivSaveInformationalRoles').show(); } }, 1000);
                    },
                    error: function (exception) {
                        try {
                            ViewModel.prototype.errorMessage("Error in adding user to the group:" + exception.responseText); //started working after changing the errorMessage from div to span. 
                            setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 15000);
                        }
                        catch (Err) {
                            alert("Error in adding user to group:" + exception.responseText);
                        }
                    }
                }).done(function (data) { });
            }
            catch (Err) {
                $(".overlay").hide();
            }
            setTimeout(function () { $(".overlay").hide(); }, 1000);
            return '';
        };

        /////////////////////////////////////////////////////////////////////////////////
        /* CHAIR SAVING   
         * Works for CouncilCommitteeGGA & Regular Councilcommitee Chair/Vice Chair Add and delete */
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectChair = function (event, ui) {
            try {
                $(".overlay").show();
                var gId = 0;
                ViewModel.prototype.errorMessage = ko.observable(); //Check on this
                if (isNaN(ViewModel.prototype.gId())) { gId = $('#hdnGroupId').val(); }
                else {
                    gId = ViewModel.prototype.gId();
                }

                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').show();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').hide();
                var company = '';
                if (ui.item.label) {
                    company = ui.item.label.trim().slice(ui.item.label.lastIndexOf('(') + 1, -1);
                    $('#hdnChairComp').val(company);
                    var txtprim = $("#Prim" + company.replace(/\s/g, ''));
                    try {
                        if ((txtprim !== null) && txtprim.val().length > 0 && $('#hdnGroupTypeId').val() != 9 && $('#hdnGroupTypeId').val() != 10 && $('#hdnGroupTypeId').val() != 1 && $('#hdnGroupTypeId').val() != 2) {
                            $('.spnMessage').text("You already have a primary representative for " + company + ".  Select another company chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').text(""); }, 6000);
                            $("#GroupChairGGA").val(""); //Clear the GGA Textbox
                            $("#GroupChair").val(""); //Clear the chair for Coun comm Textbox
                            $('#hdnChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                        else if ($("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0 && $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0) {
                            $('.spnMessage').html("You already have a primary representative and alternate representative for " + company + ".  Select another company chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                            $("#GroupChair").val(""); //Clear the textbox 
                            $('#hdnChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                    }
                    catch (Err) {
                        $(".overlay").hide();
                    }
                }
                if (!(company.length > 0)) {
                    $('.spnMessage').text("No company name found for Chair. So the selection could not be completed");
                    $(".overlay").hide();
                    return false;
                }
                var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Chair": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": company }];
                var dataObject = ko.utils.stringifyJson(temp);
                $("#hdnChkStaffUserId").val(ui.item.text);

                $.ajax({
                    url: '/api/UserGroup',
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        $('.spnMessage').html(ui.item.label + "has been successfully saved as Chair!!").css("color", "green");
                        setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                        ViewModel.prototype.DisablePrimBox();
                    },
                    error: function (exception) {
                        $('.spnMessage').html("Error in saving Group Chair" + exception.responseText).css("color", "red");
                        ui["item"]["value"] = ""; //Yay figured out how to clear autocomplete ui 
                        if ($('#hdnViceChairComp').val().indexOf($('#hdnChairComp').val()) < 0) //**Enable only if there is no vice chair from the company**
                        {
                            $("#Prim" + company.replace(/\s/g, '')).prop("disabled", false);
                            $("#Alt" + company.replace(/\s/g, '')).prop("disabled", false);//enable the primary for that company as chair not saved
                        }
                        $('#hdnChairComp').val(""); //Clear the company hidden field too
                    }
                }).done(function (data) {
                    $(".overlay").hide();
                });
            }
            catch (Err) {
                $(".overlay").hide();
            }
            $(".overlay").hide();
        };

        /////////////////////////////////////////////////////////////////////////////////
        ////////////Vice CHAIR SAVING
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectViceChair = function (event, ui) {
            try {
                $(".overlay").show();
                //Get Gid now
                var gId = 0;
                if (isNaN(ViewModel.prototype.gId())) { gId = $('#hdnGroupId').val(); }
                else
                    gId = ViewModel.prototype.gId();
                ViewModel.prototype.errorMessage = ko.observable();
                var company = '';
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').show();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').hide();
                if (ui.item.label) {
                    company = ui.item.label.trim().slice(ui.item.label.lastIndexOf('(') + 1, -1);
                    $('#hdnViceChairComp').val(company);
                    try {
                        var txtprim = $("#Prim" + company.replace(/\s/g, ''));
                        if ((txtprim !== null) && txtprim.val().length > 0 && $('#hdnGroupTypeId').val() != 9 && $('#hdnGroupTypeId').val() != 10 && $('#hdnGroupTypeId').val() != 1 && $('#hdnGroupTypeId').val() != 2) {
                            $('.spnMessage').html("You already have a primary representative for " + company + ".  Select another company vice chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                            $("#GroupViceChairGGA").val(""); //Clear the textbox 
                            $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                        else if ($("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0 && $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0) {
                            $('.spnMessage').html("You already have a primary representative and alternate representative for " + company + ".  Select another company vice chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                            $("#GroupViceChair").val(""); //Clear the textbox 
                            $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                    }
                    catch (Err) {
                    }
                }
                if (!(company.length > 0)) {
                    $('.spnMessage').html("No company name found for Vice Chair. So the selection could not be completed");
                    setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                    return;
                }

                $("#hdnChkGroupUserId").val(ui.item.text);

                var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "ViceChair": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": company }];
                var dataObject = ko.utils.stringifyJson(temp);
                $.ajax({
                    url: '/api/UserGroup',
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        $('.spnMessage').html(ui.item.label + "has been successfully saved as Vice Chair!!").css("color", "green");
                        setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                        ViewModel.prototype.DisablePrimBox();
                        $('#hdnPrimary').val();
                    },
                    error: function (exception) {
                        $('.spnMessage').html("Error in saving Group Vice Chair" + exception.responseText).css("color", "red");
                        ui["item"]["value"] = '';
                        if ($('#hdnChairComp').val().indexOf($('#hdnViceChairComp').val()) < 0) //**Enable only if there is no chair from the company**
                        {
                            $("#Prim" + company.replace(/\s/g, '')).prop("disabled", false);
                            $("#Alt" + company.replace(/\s/g, '')).prop("disabled", false); //enable the primary for that company as vice chair not saved
                        }
                        $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                    }
                }).done(function (data) {
                    $(".overlay").hide();
                });
            }
            catch (Err) { $(".overlay").hide(); }
            $(".overlay").hide();
        };

        /////////////////////////////////////////////////////////////////////////////
        /***   Participant/Informational - Add all users to Group - not being used ***/
        ////////////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectCommitteeChair = function (event, ui) {
            try {
                $(".overlay").show();
                var gId = 0;
                ViewModel.prototype.errorMessage = ko.observable(); //Check on this
                if (isNaN(ViewModel.prototype.gId())) { gId = $('#hdnGroupId').val(); }
                else {
                    gId = ViewModel.prototype.gId();
                }
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').show();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').hide();
                var company = '';
                if (ui.item.label) {
                    company = ui.item.label.trim().slice(ui.item.label.lastIndexOf('(') + 1, -1);
                    $('#hdnChairComp').val(company);
                    var txtprim = $("#Prim" + company.replace(/\s/g, ''));
                    try {
                        if ((txtprim !== null) && txtprim.val().length > 0 && $('#hdnGroupTypeId').val() != 9 && $('#hdnGroupTypeId').val() != 10 && $('#hdnGroupTypeId').val() != 1 && $('#hdnGroupTypeId').val() != 2) {
                            $('.spnMessage').text("You already have a primary representative for " + company + ".  Select another company chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').text(""); }, 6000);
                            $("#GroupChairGGA").val(""); //Clear the GGA Textbox
                            $("#GroupChair").val(""); //Clear the chair for Coun comm Textbox
                            $('#hdnChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                        else if ($("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0 && $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0) {
                            $('.spnMessage').html("You already have a primary representative and alternate representative for " + company + ".  Select another company chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                            $("#GroupChair").val(""); //Clear the textbox 
                            $('#hdnChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                    }
                    catch (Err) {
                        $(".overlay").hide();
                    }
                }
                if (!(company.length > 0)) {
                    $('.spnMessage').text("No company name found for Chair. So the selection could not be completed");
                    $(".overlay").hide();
                    return false;
                }
                var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Chair": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": company }];
                var dataObject = ko.utils.stringifyJson(temp);
                $("#hdnChkStaffUserId").val(ui.item.text);

                $.ajax({
                    url: '/api/UserGroup',
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        $('.spnMessage').html(ui.item.label + "has been successfully saved as Chair!!").css("color", "green");
                        setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                        ViewModel.prototype.DisablePrimBox();
                    },
                    error: function (exception) {
                        $('.spnMessage').html("Error in saving Group Chair" + exception.responseText).css("color", "red");
                        ui["item"]["value"] = ""; //Yay figured out how to clear autocomplete ui 
                        if ($('#hdnViceChairComp').val().indexOf($('#hdnChairComp').val()) < 0) //**Enable only if there is no vice chair from the company**
                        {
                            $("#Prim" + company.replace(/\s/g, '')).prop("disabled", false);
                            $("#Alt" + company.replace(/\s/g, '')).prop("disabled", false);//enable the primary for that company as chair not saved
                        }
                        $('#hdnChairComp').val(""); //Clear the company hidden field too
                    }
                }).done(function (data) {
                    $(".overlay").hide();
                });
            }
            catch (Err) {
                $(".overlay").hide();
            }
            $(".overlay").hide();
        };

        ViewModel.prototype.selectCommitteeViceChair = function (event, ui) {
            try {
                $(".overlay").show();
                //Get Gid now
                var gId = 0;
                if (isNaN(ViewModel.prototype.gId())) { gId = $('#hdnGroupId').val(); }
                else
                    gId = ViewModel.prototype.gId();
                ViewModel.prototype.errorMessage = ko.observable();
                var company = '';
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').show();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').hide();
                if (ui.item.label) {
                    company = ui.item.label.trim().slice(ui.item.label.lastIndexOf('(') + 1, -1);
                    $('#hdnViceChairComp').val(company);
                    try {
                        var txtprim = $("#Prim" + company.replace(/\s/g, ''));
                        if ((txtprim !== null) && txtprim.val().length > 0 && $('#hdnGroupTypeId').val() != 9 && $('#hdnGroupTypeId').val() != 10 && $('#hdnGroupTypeId').val() != 1 && $('#hdnGroupTypeId').val() != 2) {
                            $('.spnMessage').html("You already have a primary representative for " + company + ".  Select another company vice chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                            $("#GroupViceChairGGA").val(""); //Clear the textbox 
                            $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                        else if ($("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0 && $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0) {
                            $('.spnMessage').html("You already have a primary representative and alternate representative for " + company + ".  Select another company vice chair or delete the contact below.").css("color", "red");
                            setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                            $("#GroupViceChair").val(""); //Clear the textbox 
                            $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                            $(".overlay").hide();
                            return false;
                        }
                    }
                    catch (Err) {
                    }
                }
                if (!(company.length > 0)) {
                    $('.spnMessage').html("No company name found for Vice Chair. So the selection could not be completed");
                    setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                    return;
                }

                $("#hdnChkGroupUserId").val(ui.item.text);
                var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "ViceChair": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": company }];
                var dataObject = ko.utils.stringifyJson(temp);
                $.ajax({
                    url: '/api/UserGroup',
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        $('.spnMessage').html(ui.item.label + "has been successfully saved as Vice Chair!!").css("color", "green");
                        setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                        ViewModel.prototype.DisablePrimBox();
                        $('#hdnPrimary').val();
                    },
                    error: function (exception) {
                        $('.spnMessage').html("Error in saving Group Vice Chair" + exception.responseText).css("color", "red");
                        ui["item"]["value"] = '';
                        if ($('#hdnChairComp').val().indexOf($('#hdnViceChairComp').val()) < 0) //**Enable only if there is no chair from the company**
                        {
                            $("#Prim" + company.replace(/\s/g, '')).prop("disabled", false);
                            $("#Alt" + company.replace(/\s/g, '')).prop("disabled", false); //enable the primary for that company as vice chair not saved
                        }
                        $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                    }
                }).done(function (data) {
                    $(".overlay").hide();
                });
            }
            catch (Err) { $(".overlay").hide(); }
            $(".overlay").hide();
        };

        ViewModel.prototype.addGroup = function () {
            try {
                $(".overlay").show();
                var dataObject = ko.utils.stringifyJson(ViewModel.prototype.selectedValues);
                $.ajax({
                    url: '/api/UserGroup',
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        ViewModel.prototype.InfoMessage("User added to the Group Successfully!");

                        setTimeout(function () { ViewModel.prototype.InfoMessage(""); }, 3000);
                    },
                    error: function (xhr, status, error) {
                        var err = eval("(" + xhr.responseText + ")");
                        ViewModel.prototype.errorMessage("Error in Adding users. " + xhr.responseText);
                        $(".overlay").hide();
                        return false;
                    }
                });
            }
            catch (Err) { $(".overlay").hide(); }
            $(".overlay").hide();
        };

        //////////////////////////////////////////////////////////////////////////////
        /***   Participant/Informational - Delete User from UserGroup in the DB     ***/
        //////////////////////////////////////////////////////////////////////////////
        ViewModel.prototype.removeUMInfoContacts = function (contact) {
            try {
                $(".overlay").show();
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').show();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').hide();
                var gtypeid = $('#hdnGroupTypeId').val();
                var dataObject = ko.utils.stringifyJson(contact);
                dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null

                $.ajax({
                    url: '/api/RemoveGroupUser?IsA4AStaff=0&GroupTypeId=' + gtypeid,
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        ViewModel.prototype.InfoMessage("User removed from the group successfully!");
                        setTimeout(function () { ViewModel.prototype.InfoMessage(""); }, 3000);
                        ViewModel.prototype.selectedValues.remove(contact);
                    },
                    error: function (xhr, status, error) {
                        ViewModel.prototype.errorMessage("Error in removing user" + error);
                        $(".overlay").hide();
                        return false;
                    }
                });

            }
            catch (Err) { $(".overlay").hide(); }
            setTimeout(function () { $(".overlay").hide(); }, 4000);
        };

        ViewModel.prototype.removeUMContacts = function (contact) {
            try {
                $(".overlay").show();
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').show();
                $('#DivTaskGroupMsg').hide();
                var gtypeid = $('#hdnGroupTypeId').val();
                var dataObject = ko.utils.stringifyJson(contact);
                dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null

                $.ajax({
                    url: '/api/RemoveGroupUser?IsA4AStaff=0&GroupTypeId=' + gtypeid,
                    type: 'post',
                    async: false,
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        ViewModel.prototype.InfoMessage("User removed from the group successfully!");
                        setTimeout(function () { ViewModel.prototype.InfoMessage(""); }, 3000);
                        ViewModel.prototype.selectedValues.remove(contact);
                    },
                    error: function (xhr, status, error) {
                        ViewModel.prototype.errorMessage("Error in removing user" + error);
                        $(".overlay").hide();
                        return false;
                    }
                });

            }
            catch (Err) { $(".overlay").hide(); }
            setTimeout(function () { $(".overlay").hide(); }, 4000);
        };

        /////////////////////////////////////////////////////////////////////
        /***   Participant/Informational - LOAD Users from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.load = function (groupid, roleid) {
            var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid + '&RoleId=' + roleid;
            $.ajax({
                url: query,
                type: "GET",
                contentType: 'application/json',
                success: function (data) {
                    var result = JSON.parse(data);
                    for (i = 0; i < result.length; i++) {
                        var role = '';
                        var temp = {};
                        temp['Name'] = result[i].UserName;
                        temp['CompanyName'] = result[i].CompanyName;//'('+result[i].CompanyName+')';
                        temp['UserId'] = result[i].UserId;
                        temp[result[i].Role.trim()] = true;
                        temp['GroupId'] = result[i].GroupId;
                        temp['CheckStatus'] = result[i].CheckStatus;
                        ViewModel.prototype.selectedValues.push(temp);
                    }
                },
                error: function (exception) { ViewModel.prototype.errorMessage(exception.responseText); }
            });
        }

        /////////////////////////////////////////////////////
        /**Council Committee - Remove Chair /ViceChair**/
        //////////////////////////////////////////////////////
        ViewModel.prototype.removeChairGroupUser = function (item, event) {
            try {
                $(".overlay").show();
                /* Note: Reset the hidden value */
                $('#hdnChairComp').val('');
                $('#GroupChair').val('');
                $('#GroupChairGGA').val('');
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').show();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                if (isNaN(ViewModel.prototype.gId())) { gId = $('#hdnGroupId').val(); }
                else
                    gId = ViewModel.prototype.gId();
                var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 3
                //Delete the user from database and clear the field 
                var result = ViewModel.prototype.RemoveGroupUser(URL, '', 'Chair');

                $(event.target).prev().val('');
            }
            catch (Err) { $(".overlay").hide(); }
            $(".overlay").hide();
        }

        ViewModel.prototype.removeViceChairGroupUser = function (item, event) {
            try {
                $(".overlay").show();
                /* Note: Reset the hidden value */
                $('#hdnViceChairComp').val('');
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').show();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                if (isNaN(ViewModel.prototype.gId())) {
                    gId = $('#hdnGroupId').val();
                }
                else
                    gId = ViewModel.prototype.gId();

                $('#GroupViceChair').val('');
                $('#GroupViceChairGGA').val('');
                var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 4;
                /* Note: Delete the user from database and clear the field  */
                var result = ViewModel.prototype.RemoveGroupUser(URL, '', 'Vice Chair');
                $(event.target).prev().val('');
            }
            catch (Err) {
                $(".overlay").hide();
            }
            $(".overlay").hide();
        }

        ViewModel.prototype.removeCommitteeChairGroupUser = function (item, event) {
            try {
                $(".overlay").show();
                /* Note: Reset the hidden value */
                $('#hdnChairComp').val('');
                $('#GroupChair').val('');
                $('#GroupChairGGA').val('');
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').show();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                if (isNaN(ViewModel.prototype.gId())) { gId = $('#hdnGroupId').val(); }
                else
                    gId = ViewModel.prototype.gId();
                var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 3
                //Delete the user from database and clear the field 
                var result = ViewModel.prototype.RemoveGroupUser(URL, '', 'Chair');

                $(event.target).prev().val('');
            }
            catch (Err) { $(".overlay").hide(); }
            $(".overlay").hide();
        }

        ViewModel.prototype.removeCommitteeViceChairGroupUser = function (item, event) {
            try {
                $(".overlay").show();
                /* Note: Reset the hidden value */
                $('#hdnViceChairComp').val('');
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').show();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').hide();
                if (isNaN(ViewModel.prototype.gId())) {
                    gId = $('#hdnGroupId').val();
                }
                else
                    gId = ViewModel.prototype.gId();

                $('#GroupViceChair').val('');
                $('#GroupViceChairGGA').val('');
                var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 4;
                /* Note: Delete the user from database and clear the field  */
                var result = ViewModel.prototype.RemoveGroupUser(URL, '', 'Vice Chair');
                $(event.target).prev().val('');
            }
            catch (Err) {
                $(".overlay").hide();
            }
            $(".overlay").hide();
        }

        ViewModel.prototype.DisablePrimBox = function () {
            try {
                /* Note: Enabling Prim text boxes because the Chair/ViceChair user has been removed **/
                $("[id*=Prim]").prop("disabled", false);
                $("[id*=Alt]").prop("disabled", false);

                /* Note:  Disabling just the chair and vice chair company   */
                if (undefined !== $('#hdnViceChairComp').val().replace(/\s/g, '') && undefined !== $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val() && $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0) {
                    $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                }

                if (undefined !== $('#hdnViceChairComp').val().replace(/\s/g, '') && undefined !== $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val() && $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0) {
                    $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                }
                if (undefined !== $('#hdnChairComp').val().replace(/\s/g, '') && undefined !== $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).val() && $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0) {
                    $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                }
                if (undefined !== $('#hdnChairComp').val().replace(/\s/g, '') && undefined !== $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).val() && $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0) {
                    $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                }
            }
            catch (err) {
                // TODO: Thinkhow to handle this  $('.spnMessage').append("Error disabling the Primary!!");
            }
        };

        ViewModel.prototype.RemoveGroupUser = function (URL) {
            $.ajax({
                url: URL,
                type: 'post',
                async: false,
                contentType: 'application/json',
                success: function (data) {
                    ViewModel.prototype.DisablePrimBox();
                    $('.spnMessage').text("User removed successfully!!").css("color", "green");
                    setTimeout(function () { $('.spnMessage').text(""); }, 15000);
                    return true;
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    $('.spnMessage').text("Error removing user:" + xhr.responseText).css("color", "red");
                    setTimeout(function () { $('.spnMessage').text(""); }, 15000);
                    $(event.target).parent().append("<span id='" + compPrimError + "' class='text-danger'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message + "</span>");
                    return false;
                }
            });
        }

        ViewModel.prototype.selectchkStaff = function (item, event) {
            if (isNaN(ViewModel.prototype.gId())) {
                gId = $('#hdnGroupId').val();
            }
            else {
                gId = ViewModel.prototype.gId();
            }
            ChkStaff.set(gId, event.target.checked)
        }

        ViewModel.prototype.selectchkGroupUser = function (item, event) {
            if (isNaN(ViewModel.prototype.gId())) {
                gId = $('#hdnGroupId').val();
            }
            else {
                gId = ViewModel.prototype.gId();
            }
            ChkGroupUser.set(gId, event.target.checked)
        }
        return ViewModel;
    })();
    AutoCompleteExample.ViewModel = ViewModel;

})(AutoCompleteExample || (AutoCompleteExample = {}));

/////////////////////////////////////////////////////////////////////
/*******TAB 7- Taskforce & WorkGroups**************/
/////////////////////////////////////////////////////////////////////
var TaskForceWorkGroup;
(function (TaskForceWorkGroup) {
    var ViewModel = (function () {
        function ViewModel() {
            this.selectedValue = ko.observable();
            this.selectedTskForUserNW = ko.observableArray();
        }
        ViewModel.prototype.gId = ko.observable(0);
        ViewModel.prototype.edit = ko.observable(false);
        ViewModel.prototype.errorMessage = ko.observable();
        ViewModel.prototype.Roles = ko.observableArray([
            { id: "0", name: "Choose Role" },
            { id: "20", name: "Administrator" },
            { id: "3", name: "Chair" },
            { id: "15", name: "Co Chair" },
            { id: "16", name: "FAA Chair" },
            { id: "17", name: "FAA Co Chair" },
            { id: "19", name: "Observer" },
            { id: "8", name: "Secretary" },
            { id: "6", name: "Spokesperson" },
            { id: "12", name: "Participant" },
            { id: "18", name: "Team Leader" },
            { id: "4", name: "Vice Chair" }]);
        ViewModel.prototype.role = ko.observable();
        ViewModel.prototype.taskRoles = ko.observableArray();
        ViewModel.prototype.InfoMessage = ko.observable();
        ViewModel.prototype.gettaskRoles = function () {
            $.ajax({
                url: '/api/GetCommitteRolesbyGType',
                type: "GET",
                datatype: "json",
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    for (key in result) {
                        var item = {
                            name: key,         // Push the key on the array
                            value: result[key] // Push the key's value on the array
                        };
                        self.taskRoles.push(item);
                    }
                },
                error: function (exception) {
                }
            });
        };
        //Added by NA 9/20/2018 on JDs request to have a clear button clear the textboxes 
        ViewModel.prototype.clearTaskGroupUser = function () {
            $("#tskGrpUser").val("");
        };
        ViewModel.prototype.GetAllUsers = function (request, response) {
            ViewModel.prototype.errorMessage();
            var text = request.term;

            var gtypeid = $('#hdnGroupTypeId').val();
            var url = '';
            if ($('#hdnGroupTypeId').val())
                url = '/api/GetAllContacts/?grouptype=' + gtypeid;
            else
                url = '/api/GetAllContacts/';

            $.ajax({
                url: url,
                type: 'GET',
                cache: false,
                data: request,
                dataType: 'json',
                success: function (json) {
                    response($.map(json, function (key, value) {
                        return {
                            label: value, //exchanged by NA - changed the reuslt to sorted dictionary 3/26/2019
                            text: key,
                            value: ""
                        }
                    }));
                }
            });
        };
        ViewModel.prototype.selectedTskForUser = ko.observableArray();

        /////////////////////////////////////////////////////////////////////
        /***  TaskForce -  Select User to Add     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectTskFrGroupUser = function (event, ui) {
            try {
                $(".overlay").show();
                //Get Gid now
                var gId = 0;
                gId = $('#hdnGroupId').val();
                ViewModel.prototype.errorMessage = ko.observable();
                if (!isNaN(gId) || !isNaN(ViewModel.prototype.gId())) {
                    var role = ViewModel.prototype.role();
                    var roleName = $("#inpGroupRole option[value='" + role + "']").text();
                    if (typeof role === 'undefined' || ViewModel.prototype.gId() < 1 || role === "0") {
                        ViewModel.prototype.errorMessage("Role or GroupId Missing. The user cannot be added to the group.");
                    }
                    else {
                        var temp = {};
                        var cfname = ui.item.label;
                        var startindex = cfname.indexOf('(');
                        var cname = cfname.substring(startindex + 1).replace(')', ''); //Get the Company out of the Company Name.
                        var usrname = cfname.replace(cfname.substring(startindex), ''); //Get the

                        temp['Name'] = usrname;
                        temp['UserId'] = ui.item.text;
                        temp['role'] = roleName;
                        temp[roleName.replace(/\s/g, '').trim()] = true;
                        temp['CompanyName'] = cname;
                        temp['GroupId'] = ViewModel.prototype.gId();

                        var dataObject = ko.utils.stringifyJson(temp);
                        dataObject = "[" + dataObject + "]";
                        $.ajax({
                            url: '/api/UserGroup',
                            type: 'post',
                            data: dataObject,
                            contentType: 'application/json',
                            success: function (data) {
                                $("#tskErrorMsg").css("color", "Green");
                                $("#tskErrorMsg").html(ui.item.label + "has been added to the group"); // TODO: Fix it to use ViewModel.prototype.errorMessage. For somereason ViewModel.prototype.errorMessage was not updating and showing up 1/18/18
                                setTimeout(function () { $("#tskErrorMsg").html("") }, 6000);
                                ViewModel.prototype.selectedTskForUser.push(temp);
                                $(event.target).val(''); //Note: Clears the textbox after the user is added 
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $("#tskErrorMsg").css("color", "red");
                                $("#tskErrorMsg").html("Error adding user to group:" + jqXHR.responseText); // TODO: Fix it to use ViewModel.prototype.errorMessage. For somereason ViewModel.prototype.errorMessage was not updating and showing up 1/18/18
                                setTimeout(function () { $("#tskErrorMsg").html("") }, 6000);
                            }
                        });
                    }
                }
                else {
                    $("#tskErrorMsg").css("color", "red");
                    ViewModel.prototype.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
                }
            }

            catch (Err) { $(".overlay").hide(); }
            $(".overlay").hide();
        };

        /////////////////////////////////////////////////////////////////////
        /***   taskforce- Delete User from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.removeUMTaskForceContacts = function (contact) {
            try {
                $(".overlay").show();
                $('#DivMsgA4AStaffContainer').hide();
                $('#DivCompanyMsg').hide();
                $('#DivCommitteeMsg').hide();
                $('#DivInformationalContactMsg').hide();
                $('#DivSaveInformationalContactMsg').hide();
                $('#DivContactMsg').hide();
                $('#DivSaveContactMsg').hide();
                $('#DivTaskGroupMsg').show();
                var dataObject = ko.utils.stringifyJson(contact);
                ViewModel.prototype.errorMessage();
                dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
                $.ajax({
                    url: '/api/RemoveGroupUser',
                    type: 'post',
                    data: dataObject,
                    contentType: 'application/json',
                    success: function (data) {
                        $("#tskErrorMsg").css("color", "green");
                        ViewModel.prototype.errorMessage(contact.Name + " has been removed successfully!");
                        setTimeout(function () { ViewModel.prototype.errorMessage() }, 6000);

                    },
                    error: function (xhr, status, error) {
                        $("#tskErrorMsg").css("color", "red");
                        ViewModel.prototype.errorMessage("Error in removing" + contact.Name + xhr.responseText);
                        setTimeout(function () { ViewModel.prototype.errorMessage() }, 6000);
                        $(".overlay").hide();
                        return false;
                    }
                });
                ViewModel.prototype.selectedTskForUser.remove(contact);
            }
            catch (Err) {
                ViewModel.prototype.errorMessage("Error in removing" + contact.Name + Err);
                $(".overlay").hide();
            }
            $(".overlay").hide();
        };

        /////////////////////////////////////////////////////////////////////
        /***   taskforce- LOAD Users from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.load = function (groupid) {
            var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid;
            ViewModel.prototype.errorMessage();
            $.ajax({
                url: query,
                type: "GET",
                contentType: 'application/json',
                success: function (data) {
                    var result = JSON.parse(data);
                    for (i = 0; i < result.length; i++) {
                        var temp = {};
                        temp['Name'] = result[i].UserName;
                        temp['CompanyName'] = result[i].CompanyName; //Removed (  )  because we made company seperate field
                        temp['UserId'] = result[i].UserId;
                        temp['role'] = result[i].Role;
                        temp[result[i].Role.replace(/\s/g, '').trim()] = true;
                        temp['GroupId'] = result[i].GroupId;
                        ViewModel.prototype.selectedTskForUser.push(temp);
                    }
                },
                error: function (exception) {
                    $("#tskErrorMsg").css("color", "red");
                    ViewModel.prototype.errorMessage('Exception in loading task force user list:' + exception.responseText);
                }
            });
        }
        return ViewModel;
    })();
    TaskForceWorkGroup.ViewModel = ViewModel;

})(TaskForceWorkGroup || (TaskForceWorkGroup = {}));

ko.observableArray.fn.distinct = function (prop) {
    var target = this;
    target.index = {};
    target.index[prop] = ko.observable({});

    ko.computed(function () {
        //rebuild index
        var propIndex = {};

        ko.utils.arrayForEach(target(), function (item) {
            var key = ko.utils.unwrapObservable(item[prop]);
            if (key) {
                propIndex[key] = propIndex[key] || [];
                propIndex[key].push(item);
            }
        });
        target.index[prop]
        target.index[prop](propIndex);
    });

    return target;
};

/////////////////////////////////////////////////////////////////////
/*******Tab 4 - Council/Committee View Model - FINAL********/
/////////////////////////////////////////////////////////////////////
function councilCommitteeViewModel(params) {
    self = this;
    self.Company = ko.observable(params && params.Company || '1');
    self.errorPrimUser = ko.observable();
    self.errorAltUser = ko.observable();
    var t1 = sessionStorage.getItem("Prim" + params.Company.replace(/\s/g, ''));
    var t2 = sessionStorage.getItem("Alt" + params.Company.replace(/\s/g, ''));
    self.PrimUserValue = ko.observable(t1);
    self.AltUserValue = ko.observable(t2);
    self.PrimaryID = ko.observable("Prim" + params.Company.replace(/\s/g, ''));
    self.AlternateID = ko.observable("Alt" + params.Company.replace(/\s/g, ''));
    var CheckStatusPrimary = GetCompanyNamePrimary.get("Prim" + params.Company.replace(/\s/g, ''));
    var CheckStatusAlternate = GetCompanyNameAlternate.get("Alt" + params.Company.replace(/\s/g, ''));
    self.CheckStatusPrimary = ko.observable(CheckStatusPrimary);
    self.CheckStatusAlternate = ko.observable(CheckStatusAlternate);
    self.ChkPrimaryID = ko.observable("ChkPrim" + params.Company.replace(/\s/g, ''));
    self.ChkAlternateID = ko.observable("ChkAlt" + params.Company.replace(/\s/g, ''));

    DisablePrimary();

    var gId = $('#hdnGroupId').val() || parseInt(getGID('gid'));
    self.gId = ko.observable(gId);
    self.errorMessage = ko.observable();

    function getGID(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'), sParameterName, i;
        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    }
    if (!(gId > 0)) {
        $("#errorPrimUser").val("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
    } else {
        $("#errorPrimUser").val("");
    }
    /********************************************/
    /* Council/Committee- Databind Primary & Secondary */
    /********************************************/
    self.CompanyUsers = ko.observableArray();
    self.getCompanyUsers = function (request, response) {
        var text = request.term;
        $.ajax({
            url: '/api/GetComapnyUsers/?CompanyName=' + params.Company, //term=' + request.term +
            type: 'GET',
            cache: false,
            data: request,
            dataType: 'json',
            success: function (json) {
                response($.map(json, function (key, value) {
                    return {
                        label: value,
                        text: key,
                        value: ""
                    }
                }));
            }
        });
    };
    self.closeSelect = function (event, ui) {
    }
    self.selectedValues = ko.observableArray();
    self.selectPrimary = function (event, ui) {
        try {
            $(".overlay").show()
            var compPrimError = (params.Company).replace(/\s+/g, '') + "PrimError";
            /**Remove Error Message*/
            var primErrItem = $('#' + compPrimError);
            if (typeof primErrItem !== "undefined")
                primErrItem.remove();
            $('#DivMsgA4AStaffContainer').hide();
            $('#DivCompanyMsg').show();
            $('#DivCommitteeMsg').hide();
            $('#DivInformationalContactMsg').hide();
            $('#DivSaveInformationalContactMsg').hide();
            $('#DivContactMsg').hide();
            $('#DivSaveContactMsg').hide();
            $('#DivTaskGroupMsg').hide();
            //Get GroupId
            if (!(gId > 0)) //Added on 042018 because alternate was not getting gId value //added here for uniformity
                gId = $('#hdnGroupId').val() || parseInt(getGID('gid'));
            /**Build the data to be posted**/
            var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Primary": true, GroupId: gId, CompanyName: params.Company }];
            var dataObject = ko.utils.stringifyJson(temp);

            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                    $('.spnMessage').html(ui.item.label + " has been successfully added as Primary for " + params.Company).css("color", "green");
                    setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                    if (undefined !== $('#hdnChairComp').val() && $('#hdnChairComp').val().replace(/\s/g, '').length > 0 && params.Company === $('#hdnChairComp').val()) {
                        $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                    }
                    if (undefined !== $('#hdnViceChairComp').val() && $('#hdnViceChairComp').val().replace(/\s/g, '').length > 0 && params.Company === $('#hdnViceChairComp').val()) {
                        $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                    }
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    var par = $(event.target).parent();
                    par.append("<div id='" + compPrimError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message); // + " Please clear it manually by selecting Ctrl + A in the box. And try again</div>"
                    $("#Prim" + (params.Company).replace(/\s+/g, '')).val("");
                    console.log("In Primary" + $("#Prim" + (params.Company).replace(/\s+/g, '')).val());
                    $(".overlay").hide();
                    return false;
                }
            });
            /* In case of cancelling the selection because of invalid user selection*/
            var loc = $(this).val().indexOf('all')
            if (loc > -1 && loc < 1) { $(this).val(''); $(".overlay").hide(); return false; }
            else
                $(this).val(ui.item.label);
        }
        catch (Err) {
            $(".overlay").hide();
        }
        $(".overlay").hide();
    };
    self.removePrimGroupUser = function (item, event) {
        $(".overlay").show();
        if (params.Company.length > 0 && !isNaN(gId)) {
            var URL = "/api/RemoveGrpUsrbyComp/?CompanyName=" + params.Company + "&GroupId=" + gId + "&RoleId=" + 1;
            //Delete the user from database and clear the field
            RemoveGroupUser(URL, params.Company, "Primary");
            var compPrimError = (params.Company).replace(/\s+/g, '') + "PrimError";
            var PrimErrItem = $('#' + compPrimError);
            if (typeof compPrimError !== "undefined" && typeof PrimErrItem !== "undefined") { //to prevent error on hitting on empty box
                PrimErrItem.remove();
            }
            $(event.target).prev().val('');
        }
        $('#DivMsgA4AStaffContainer').hide();
        $('#DivCompanyMsg').show();
        $('#DivCommitteeMsg').hide();
        $('#DivInformationalContactMsg').hide();
        $('#DivSaveInformationalContactMsg').hide();
        $('#DivContactMsg').hide();
        $('#DivSaveContactMsg').hide();
        $('#DivTaskGroupMsg').hide();
    }
    self.removeAltGroupUser = function (item, event) {
        $(".overlay").show();
        if (params.Company.length > 0 && !isNaN(gId)) {
            var URL = "/api/RemoveGrpUsrbyComp/?CompanyName=" + params.Company + "&GroupId=" + gId + "&RoleId=" + 2;
            //Delete the user from database and clear the field
            RemoveGroupUser(URL, params.Company, "Alternate");
            $(event.target).prev().val('');
            var compAltError = (params.Company).replace(/\s+/g, '') + "PrimAlt";
            /**Remove Error Message*/
            var altErrItem = $('#' + compAltError);
            if (typeof compAltError !== "undefined" && typeof altErrItem !== "undefined")
                altErrItem.remove();
        }
        $('#DivMsgA4AStaffContainer').hide();
        $('#DivCompanyMsg').show();
        $('#DivCommitteeMsg').hide();
        $('#DivInformationalContactMsg').hide();
        $('#DivSaveInformationalContactMsg').hide();
        $('#DivContactMsg').hide();
        $('#DivSaveContactMsg').hide();
        $('#DivTaskGroupMsg').hide();
    }
    function RemoveGroupUser(URL, Company, Role) {
        $.ajax({
            url: URL,
            type: 'post',
            async: false,
            contentType: 'application/json',
            success: function (data) {
                $('.spnMessage').html("Removed " + Role + " for " + Company).css("color", "green");
                setTimeout(function () { $('.spnMessage').html(""); }, 6000);

                if (undefined !== $('#hdnChairComp').val() && $('#hdnChairComp').val() === Company && $('#hdnChairComp').val().replace(/\s/g, '').length > 0) {
                    if (Role === 'Alternate') {
                        $('#Prim' + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", false);
                    } else {
                        $('#Alt' + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", false);
                    }
                }
                if (undefined !== $('#hdnViceChairComp').val() && $('#hdnViceChairComp').val() === Company && $('#hdnViceChairComp').val().replace(/\s/g, '').length > 0) {
                    if (Role === 'Alternate') {
                        $('#Prim' + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", false);
                    } else {
                        $('#Alt' + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", false);
                    }
                }
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                if (!(typeof compPrimError === "undefined")) {
                    $(event.target).parent().append("<span id='" + compPrimError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message + "</span>");
                }
                else {
                    $('.spnMessage').html("Error in removing " + Role + " for " + Company + ". Error: " + err.Message).css("color", "red");
                    setTimeout(function () { $('.spnMessage').html(""); }, 6000);
                }
                $(".overlay").hide();
                return false;
            }
        })
    }
    self.selectAlternate = function (event, ui) {
        try {
            $(".overlay").show();
            var compAltError = (params.Company).replace(/\s+/g, '') + "PrimAlt";
            /**Remove Error Message*/
            var altErrItem = $('#' + compAltError);
            if (typeof altErrItem !== "undefined")
                altErrItem.remove();

            //Get GroupId 
            if (!(gId > 0))
                gId = $('#hdnGroupId').val() || parseInt(getGID('gid'));

            $('#DivMsgA4AStaffContainer').hide();
            $('#DivCompanyMsg').show();
            $('#DivCommitteeMsg').hide();
            $('#DivInformationalContactMsg').hide();
            $('#DivSaveInformationalContactMsg').hide();
            $('#DivContactMsg').hide();
            $('#DivSaveContactMsg').hide();
            $('#DivTaskGroupMsg').hide();

            var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Alternate": true, GroupId: gId, CompanyName: params.Company }];
            var dataObject = ko.utils.stringifyJson(temp);
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                    $('.spnMessage').html(ui.item.label + " has been successfully added as Alternate for " + params.Company).css("color", "green");

                    if (undefined !== $('#hdnChairComp').val() && $('#hdnChairComp').val().replace(/\s/g, '').length > 0 && params.Company === $('#hdnChairComp').val()) {
                        $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                    }

                    if (undefined !== $('#hdnViceChairComp').val() && $('#hdnViceChairComp').val().replace(/\s/g, '').length > 0 && params.Company === $('#hdnViceChairComp').val()) {
                        $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
                    }
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    $(event.target).parent().append("<div id='" + compAltError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message); //+ " Please clear it manually by selecting Ctrl + A in the box. And try again</div>"
                    $("#Alt" + (params.Company).replace(/\s+/g, '')).val("");
                    console.log("In Alternate" + $("#Prim" + (params.Company).replace(/\s+/g, '')).val());
                    $(".overlay").hide();
                    return false;
                }
            })
        }
        catch (Err) {
            $(".overlay").hide();
        } $(".overlay").hide();
    };

    this.selectCompanyNamePrimary = function (event, ui) {
        if (params.Company.length > 0 && !isNaN(gId)) {
            CompanyNamePrimary.set(params.Company, ui.target.checked)
        }
    };

    this.selectCompanyNameAlternate = function (event, ui) {
        if (params.Company.length > 0 && !isNaN(gId)) {
            CompanyNameAlternate.set(params.Company, ui.target.checked)
        }
    };
}
/// END OF Council/Committee
/////////////////////////////////////////////////////////////////////////////

// BEgin Council/Committee GGA
///////////////////////////////////////////////////////////

///////////////////////////////////////
//GGA CommitteePrimAlt
////////////////////////////////////// 
var CommitteePrimAlt = function (Name, UserId, Type, CompanyName, Alternate, Primary, Informational, CheckStatus) {
    this.Name = ko.observable(Name);
    this.UserId = ko.observable(UserId);
    this.CompanyName = ko.observable(CompanyName);
    this.Type = ko.observable(Type);
    this.Alternate = ko.observable(Alternate);
    this.Primary = ko.observable(Primary);
    this.Informational = ko.observable(Informational);
    this.CheckStatus = ko.observable(CheckStatus);
}

var councilCommitteeGGAViewModel = function () {
    var self = this;
    self.gId = ko.observable($('#hdnGroupId').val());
    var gId = $('#hdnGroupId').val();
    self.errorMessage = ko.observable();
    self.infoMessage = ko.observable();
    self.people = ko.observableArray([]).distinct('CompanyName');
    self.Roles = ko.observableArray([
        { id: "2", name: "Alternate" },
        { id: "1", name: "Primary" },
        { id: "5", name: "Informational" }]);
    self.role = ko.observable();
    self.Companies = ko.observableArray();
    self.company = ko.observable();
    self.userSelected = ko.observable();
    self.userSelectedId = ko.observable();
    self.GetCompanies = $.getJSON('/api/GetCompanies/?isMember=' + true, function (data) {
        $.each(data, function (key, value) {
            self.Companies.push({ id: value, name: key });
        });
    });

    /**
     * **************************
     * Used in A4A.UserManagement\Views\Group\_CouncilCommitteeGGA.cshtml 
     * For adding and deleting users
     */
    this.selectPrimAltUser = function (event, ui) {
        self.userSelected(ui.item.label);
        self.userSelectedId(ui.item.text);
    }
    this.selectPrimAltUser4Company = function (event, ui) {
        if (!isNaN($('#hdnGroupId').val())) {
            self.gId($('#hdnGroupId').val());
            gId = $('#hdnGroupId').val();
        }
        if ((isNaN(gId) || (isNaN(self.gId())))) {
            self.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
            setTimeout(function () { self.errorMessage(""); }, 6000);
        }
        else {
            var UsernCompname = self.userSelected();
            var startindex = UsernCompname.indexOf('(');
            var CompanyName = UsernCompname.substring(startindex + 1).replace(')', '').trim(); //Get the Company out of the Company Name string.
            var usrname = UsernCompname.replace(UsernCompname.substring(startindex), ''); //Get the companyname out of the username
            var role = self.role();
            var roleName = $("#inpGGAGroupRole option[value='" + role + "']").text();
            var comp = CompanyName;
            if (($('#hdnGroupTypeId').val() != 9 && $('#hdnGroupTypeId').val() != 10) && roleName.toLowerCase().trim().indexOf('primary') !== -1 && ($('#hdnChairComp').val().trim().indexOf(CompanyName) !== -1 || $('#hdnViceChairComp').val().trim().indexOf(CompanyName) !== -1)) {
                self.errorMessage("This company already has a representative assigned for Chair/ViceChair. So cannot add Primary!"); //Removed the alternate and added clause check only for adding primary users if a chair/vice chair is selected
                setTimeout(function () { self.errorMessage(""); }, 6000);
            }
            else {
                //Set the item to push to DB & knockout array
                var temp = {};
                temp['Name'] = usrname;
                temp['UserId'] = self.userSelectedId();
                temp['Type'] = roleName;
                temp[roleName.replace(/\s/g, '').trim()] = true;
                temp['GroupId'] = self.gId();
                temp['CompanyName'] = CompanyName;
                var dataObject = ko.utils.stringifyJson(temp);
                dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null

                //First check if the user already exists in the array
                var match = ko.utils.arrayFirst(self.people(),
                    function (item) {
                        return self.userSelectedId() == item.UserId();
                    });
                if (!match) {
                    if (roleName === "Primary") {
                        var PrimaryExists = false;
                        var k = $.getJSON('/api/checkifcompanyhasrole?GroupId=' + self.gId() + "&CompanyName=" + CompanyName + "&RoleId=" + 1, function (data) {
                            PrimaryExists = data
                        }).done(
                            function () {
                                if (!PrimaryExists || $('#hdnGroupTypeId').val() == 9 || $('#hdnGroupTypeId').val() == 10) {
                                    $.ajax({
                                        url: '/api/UserGroupGGA',
                                        type: 'post',
                                        data: dataObject,
                                        contentType: 'application/json',
                                        success: function (data) {
                                            self.people.push(new CommitteePrimAlt(usrname, self.userSelectedId(), roleName, CompanyName, true, false, false, false));
                                            self.infoMessage("Successfully added a Primary user to " + CompanyName);
                                            if ($('#hdnchkRadioId').val() != "1") {
                                                $("input[name='EmailAdmin']").prop("disabled", true);
                                                $('#DivSaveCommitteeRoles').hide();
                                            }
                                            else {
                                                $("input[name='EmailAdmin']").prop("disabled", false);
                                                $('#DivSaveCommitteeRoles').show();
                                            }
                                            $('#DivRemoveCommitteePrimAlt').find("a").show();
                                            setTimeout(function () { self.infoMessage(""); }, 6000);
                                            return false;
                                        },
                                        error: function (xhr, status, error) {
                                            self.errorMessage("Error in adding user: " + xhr.responseText);
                                            self.role("");
                                            setTimeout(function () { self.errorMessage(""); }, 6000);
                                        }
                                    });
                                }
                                else {
                                    self.errorMessage("A primary has already been set for this " + CompanyName + ".Please delete the current primary and then add this user!");
                                    setTimeout(function () { self.errorMessage(""); }, 6000);
                                    $("#ggaUser").val("");
                                    self.role("");
                                    self.company(null);
                                }
                            });
                    }
                    else if (roleName === "Informational") {
                        $.ajax({
                            url: '/api/UserGroupGGA',
                            type: 'post',
                            data: dataObject,
                            contentType: 'application/json',
                            success: function (data) {
                                self.people.push(new CommitteePrimAlt(usrname, self.userSelectedId(), roleName, CompanyName, false, false, true, false));
                                self.infoMessage("Successfully added an Informational user to " + CompanyName);
                                if ($('#hdnchkRadioId').val() != "1") {
                                    $("input[name='EmailAdmin']").prop("disabled", true);
                                    $('#DivSaveCommitteeRoles').hide();
                                }
                                else {
                                    $("input[name='EmailAdmin']").prop("disabled", false);
                                    $('#DivSaveCommitteeRoles').show();
                                }
                                $('#DivRemoveCommitteePrimAlt').find("a").show();
                                setTimeout(function () { self.infoMessage(""); }, 6000);
                                return false;
                            },
                            error: function (xhr, status, error) {
                                self.errorMessage("Error in adding user: " + xhr.responseText);
                                self.role("");
                                setTimeout(function () { self.errorMessage(""); }, 6000);
                            }
                        });
                    }
                    else if (roleName === "Alternate") {
                        $.ajax({
                            url: '/api/UserGroupGGA',
                            type: 'post',
                            data: dataObject,
                            contentType: 'application/json',
                            success: function (data) {
                                self.people.push(new CommitteePrimAlt(usrname, self.userSelectedId(), roleName, CompanyName, false, true, false, false));
                                self.infoMessage("Successfully added an Alternate user to " + CompanyName);
                                if ($('#hdnchkRadioId').val() != "1") {
                                    $("input[name='EmailAdmin']").prop("disabled", true);
                                    $('#DivSaveCommitteeRoles').hide();
                                }
                                else {
                                    $("input[name='EmailAdmin']").prop("disabled", false);
                                    $('#DivSaveCommitteeRoles').show();
                                }
                                $('#DivRemoveCommitteePrimAlt').find("a").show();
                                setTimeout(function () { self.infoMessage(""); }, 6000);
                                return false;
                            },
                            error: function (xhr, status, error) {
                                self.errorMessage("Error in adding user: " + xhr.responseText);
                                self.role("");
                                setTimeout(function () { self.errorMessage(""); }, 6000);
                            }
                        });
                    }
                }
                else {
                    self.errorMessage("The user " + match.Name() + " already exists under " + match.CompanyName() + " ");
                    setTimeout(function () { self.errorMessage(""); }, 6000);
                }

                $("#ggaUser").val('');
                self.role("");
            }
        }
    };

    self.removeCommitteePrimAlt = function (CommitteePrimAlt) {
        $(".overlay").show();
        if (isNaN(self.gId())) {
            self.errorMessage("GroupId is missing. The user cannot be removed from the group.");
            setTimeout(function () { self.errorMessage(""); }, 6000);
            return false;
        }
        if (isNaN(CommitteePrimAlt.UserId())) {
            self.errorMessage("User Information is missing. The user cannot be removed from the group. Please contact IT");
            setTimeout(function () { self.errorMessage(""); }, 6000);
            return false;
        }
        else {
            self.people.remove(CommitteePrimAlt);
            var role = 1;
            if (CommitteePrimAlt.Type() === "Alternate")
                role = 2;
            if (CommitteePrimAlt.Type() === "Informational")
                role = 5;
            var URL = "/api/RemoveGrpUsrbyUserId/?UserId=" + CommitteePrimAlt.UserId() + "&GroupId=" + self.gId() + "&RoleId=" + role;
            RemoveGroupUser(URL);
            $(".overlay").hide();
            return false;
        }
    };
    function RemoveGroupUser(URL) {
        $.ajax({
            url: URL,
            type: 'post',
            async: false,
            contentType: 'application/json',
            success: function (data) {
                self.infoMessage("User removed successfully!");
                setTimeout(function () { self.infoMessage(""); }, 6000);
                return true;
            },
            error: function (xhr, status, error) {
                self.errorMessage("Error occured in remove group user!" + xhr.responseText);
                setTimeout(function () { self.errorMessage(""); }, 6000);
                $(".overlay").hide();
                return false;
            }
        })
    }

    //Added by NA - 9/20/2018 on JDs request to have a clear button clear the textboxes 
    this.clearGGAUser = function () {
        $("#ggaUser").val("");
        self.userSelected(null);
        self.userSelectedId(null);
    };
    this.GetAllUsers = function (request, response) {
        var text = request.term;
        var comp = self.company();
        var CompanyName = $("#inpGGACompanies option[value='" + comp + "']").text();
        $.ajax({
            url: '/api/GetComapnyUsers/?CompanyName=' + CompanyName,
            type: 'GET',
            cache: false,
            data: request,
            dataType: 'json',
            success: function (json) {
                response($.map(json, function (key, value) {
                    return {
                        label: value,
                        text: key,
                        value: ""
                    };
                }));
            }
        });
    };

    this.GetAllUserWithCompany = function (request, response) {
        var text = request.term;
        var comp = self.company();
        var CompanyName = $("#inpGGACompanies option[value='" + comp + "']").text();
        var gtypeid = $('#hdnGroupTypeId').val();
        var url = '';
        if ($('#hdnGroupTypeId').val())
            url = '/api/GetAllContacts/?grouptype=' + gtypeid;
        else
            url = '/api/GetAllContacts/';
        $.ajax({
            url: url,
            type: 'GET',
            cache: false,
            data: request,
            dataType: 'json',
            success: function (json) {
                response($.map(json, function (key, value) {
                    return {
                        label: value,
                        text: key,
                        value: ""
                    };
                }));
            }
        });
    };
    /////////////////////////////////////////////////////////////////////
    /***   Council Committe GGA- LOAD Users from UserGroup in the DB     ***/
    ////////////////////////////////////////////////////////////////////
    self.load = function (groupid) {
        var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid + "&RoleId=1,2,3,4";
        self.errorMessage("");
        $.ajax({
            url: query,
            type: "GET",
            contentType: 'application/json',
            success: function (data) {
                var result = JSON.parse(data);
                for (i = 0; i < result.length; i++) {
                    if (result[i].RoleId === 1) {
                        self.people.push(new CommitteePrimAlt(result[i].UserName, result[i].UserId, result[i].Role, result[i].CompanyName, true, false, false, result[i].CheckStatus));
                    }
                    else
                        if (result[i].RoleId === 2) {
                            self.people.push(new CommitteePrimAlt(result[i].UserName, result[i].UserId, result[i].Role, result[i].CompanyName, false, true, false, result[i].CheckStatus));
                        }
                        else
                            if (result[i].RoleId === 3) {
                                $('#GroupChairGGA').val(result[i].UserName + ' (' + result[i].CompanyName + ')');
                                $('#hdnChairComp').val(result[i].CompanyName);
                                DisablePrimary();

                            }
                            else
                                if (result[i].RoleId === 4) {
                                    $('#GroupViceChairGGA').val(result[i].UserName + ' (' + result[i].CompanyName + ')');
                                    $('#hdnViceChairComp').val(result[i].CompanyName);
                                    DisablePrimary();
                                }
                }
            },
            error: function (exception) {
                self.errorMessage('Exception in loading task force user list:' + exception.responseText);
                setTimeout(function () { self.errorMessage(""); }, 6000);
            }
        });

        var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid + '&RoleId=5';
        $.ajax({
            url: query,
            type: "GET",
            contentType: 'application/json',
            success: function (data) {
                var result = JSON.parse(data);
                for (i = 0; i < result.length; i++) {
                    self.people.push(new CommitteePrimAlt(result[i].UserName, result[i].UserId, result[i].Role, result[i].CompanyName, false, false, true, result[i].CheckStatus));
                }
            },
            error: function (exception) { ViewModel.prototype.errorMessage(exception.responseText); }
        });
    };
};
function DisablePrimary() {
    try {
        $('#z-tab3').find('input, button').prop('disabled', true);
        var obj = $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, ''));
        if (undefined !== $('#hdnViceChairComp').val().replace(/\s/g, '') && undefined !== $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val() && $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0) {
            $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
        }
        if (undefined !== $('#hdnViceChairComp').val().replace(/\s/g, '') && undefined !== $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val() && $("#Alt" + $('#hdnViceChairComp').val().replace(/\s/g, '')).val().length > 0) {
            $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
        }
        if (undefined !== $('#hdnChairComp').val().replace(/\s/g, '') && undefined !== $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).val() && $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0) {
            $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
        }
        if (undefined !== $('#hdnChairComp').val().replace(/\s/g, '') && undefined !== $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).val() && $("#Alt" + $('#hdnChairComp').val().replace(/\s/g, '')).val().length > 0) {
            $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
        }
    }
    catch (err) {
        // TODO: Thinkhow to handle this  $('.spnMessage').append("Error disabling the Primary!!");
    }
}

/////////////////////////////////////////////////////////////////////
/*******Global KO binding Object********/
/////////////////////////////////////////////////////////////////////
var A4AStaffViewModel = new A4AStaffModel();
var UMSubscribe = new AutoCompleteExample.ViewModel();
$(document).ready(function () {
    //Clear session 
    sessionStorage.clear();
    getManagedGroupCount(parseInt(getGID('gid'), 0));
    $("#breadcrumb ul").append('<li><a href="/group/list/"> Group List </a></li>');

    // Register knockout UI components
    ko.components.register('primaryalternate', {
        viewModel: councilCommitteeViewModel,
        template: "<div class='row' style='margin-bottom:8px;'> <div class='col-sm-2'> <span class='niceLabel' data-bind='text: Company'></span></div>" +
            "<div class='col-sm-1' style='text-align:center'><input type='checkbox' name='EmailAdmin' style='margin-right:10px' data-bind='attr: { id:ChkPrimaryID()}, event:{ change: selectCompanyNamePrimary}, checked:CheckStatusPrimary' /></div>" +
            "<div class='col-sm-3'><input type='text' class='P1 chosen form-control' style='width:300px;' data-bind='value:PrimUserValue,attr: { id:PrimaryID()},ko_autocomplete: { source: getCompanyUsers, select: selectPrimary ,minLength: 3,close: closeSelect }' />" +
            "<a href='#' class='text-danger btndelete' style='vertical-align: middle;padding-left:7px;' data-bind='click: $data.removePrimGroupUser'>Delete</a></div>" +
            "<div class='col-sm-1' style='text-align:center'><input type='checkbox' name='EmailAdmin' style='margin-right:10px' data-bind='attr: { id:ChkAlternateID()}, event:{ change: selectCompanyNameAlternate}, checked:CheckStatusAlternate' /></div>" +
            "<div class='col-sm-3'><input type='text' class='A1 chosen form-control' style='width:300px;' data-bind='value:AltUserValue,attr: { id:AlternateID() },ko_autocomplete: { source: getCompanyUsers, select: selectAlternate ,minLength:3,close: closeSelect }'/>" +
            "<a href='#' class='text-danger btndelete' style='vertical-align: middle;padding-left:7px;' data-bind='click: removeAltGroupUser'>Delete</a></div></div>"
    });

    function getGID(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'), sParameterName, i;
        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    }

    function SetPrimaryAlternate(groupid, roleid) {
        var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid;
        $.ajax({
            url: query,
            type: "GET",
            contentType: 'application/json',
            success: function (data) {
                var result = JSON.parse(data);
                for (i = 0; i < result.length; i++) {
                    try {
                        var Name = result[i].UserName;
                        var t = result[i].CompanyName.replace(/\s/g, '');

                        if (result[i].RoleId === 1) {
                            sessionStorage.setItem("Prim" + t, Name);
                            GetCompanyNamePrimary.set("Prim" + t, result[i].CheckStatus);
                        }
                        else
                            if (result[i].RoleId === 2) {
                                sessionStorage.setItem("Alt" + t, Name);
                                GetCompanyNameAlternate.set("Alt" + t, result[i].CheckStatus);
                            }
                            else
                                if (result[i].RoleId === 3) {
                                    $('#GroupChair').val(Name + ' (' + t + ')');
                                    $('#hdnChairComp').val(result[i].CompanyName);
                                    CheckStatusStaff(result[i].CheckStatus);
                                    $("#hdnChkStaffUserId").val(result[i].UserId);
                                }
                                else
                                    if (result[i].RoleId === 4) {
                                        6
                                        $('#GroupViceChair').val(Name + ' (' + t + ')');
                                        $('#hdnViceChairComp').val(result[i].CompanyName);
                                        $("#hdnChkGroupUserId").val(result[i].UserId);
                                        CheckStatusGroupUser(result[i].CheckStatus);
                                    }
                    }
                    catch (err) {
                        console.log("error in setting primary/Alternate" + err);
                    }
                }
            },
            error: function (exception) {
            }
        });
    }
    ko.validation.init({
        insertMessages: false
    });

    if (parseInt(getGID('gid')) > 0) {
        enableForm();
        groupRegisterViewModel = {
            addGroupViewModel: new Group(), addA4AStaffViewModel: A4AStaffViewModel,
            UMSubscribeModel: UMSubscribe, addTaskForceWorkGroup: new TaskForceWorkGroup.ViewModel(),
            addCouncilCommitteeGGA: new councilCommitteeGGAViewModel()
        };

        $.when(SetPrimaryAlternate(parseInt(getGID('gid'), 0))).done(setTimeout(function () { ko.applyBindings(groupRegisterViewModel); }, 1300));
        groupRegisterViewModel.addGroupViewModel.errors = ko.validation.group(groupRegisterViewModel.addGroupViewModel);

        $.when(groupRegisterViewModel.addGroupViewModel.getData1(), groupRegisterViewModel.addGroupViewModel.getData2(),
            groupRegisterViewModel.addGroupViewModel.getDepartments(),
            groupRegisterViewModel.addGroupViewModel.getDataGroupTypes(),
            groupRegisterViewModel.addGroupViewModel.getDataLiaisons(),
            groupRegisterViewModel.addA4AStaffViewModel.getA4AStaffList(), groupRegisterViewModel.addTaskForceWorkGroup.gettaskRoles()).done();
    }
    else {
        groupRegisterViewModel = {
            addGroupViewModel: new Group(), addA4AStaffViewModel: A4AStaffViewModel,
            UMSubscribeModel: UMSubscribe, addTaskForceWorkGroup: new TaskForceWorkGroup.ViewModel(),
            addCouncilCommitteeGGA: new councilCommitteeGGAViewModel()
        };
        disableForm();
        ko.applyBindings(groupRegisterViewModel);

        groupRegisterViewModel.addGroupViewModel.errors = ko.validation.group(groupRegisterViewModel.addGroupViewModel);

        $.when(groupRegisterViewModel.addGroupViewModel.getData1(), groupRegisterViewModel.addGroupViewModel.getData2(),
            groupRegisterViewModel.addGroupViewModel.getDepartments(),
            groupRegisterViewModel.addGroupViewModel.getDataGroupTypes(),
            groupRegisterViewModel.addGroupViewModel.getDataLiaisons(),
            groupRegisterViewModel.addA4AStaffViewModel.getA4AStaffList(),
            groupRegisterViewModel.addTaskForceWorkGroup.gettaskRoles()).done();
    }
});

function getManagedGroupCount(groupid) {
    $.getJSON('/api/getManageGroupCount?groupId=' + groupid, function (data) {
        manageGroup = data;
    }).done(function (data) {
        $.getJSON('/api/GetIsCurrentUserAdmin', function (data) {
            isAdmin(data);
        }).done(function (data) {
            if (!data) {
                if (manageGroup <= 1) {
                    shouldShowDelete(false);
                }
            }
        });
    });
}

function disableForm() {
    $('#z-tab2, #z-tab3, #z-tab4, #z-tab5, #z-tab6, #z-tab7').find('input, button, submit, textarea, select').prop('disabled', true);
    $('#z-tab2, #z-tab3, #z-tab4, #z-tab5, #z-tab6, #z-tab7').find('button, a').hide();
}

function enableForm() {
    $('#z-tab2, #z-tab3, #z-tab4, #z-tab5, #z-tab6, #z-tab7').find('input, button, submit, textarea, select').prop('disabled', false);
    $('#z-tab2, #z-tab3, #z-tab4, #z-tab5, #z-tab6, #z-tab7').find('button, a').show();
    $('.btndelete').css('display', 'inline-block !important');
    $('#z-tab3').find('input').prop('disabled', false);
}

function handleClick(myRadio) {
    if ($('#hdnSaveGroupValue').val() == "1") {
        if (myRadio.value != "1") {
            $("input[name='EmailAdmin']").prop("disabled", true);
            $('#DivSaveInformationalRoles').hide();
            $('#DivSaveContactsRoles').hide();
            $('#DivSaveCompanyRoles').hide();
            $('#DivSaveCommitteeRoles').hide();
            $('#DivSaveTaskGroupRoles').hide();
        }
        else {
            $("input[name='EmailAdmin']").prop("disabled", false);
            $('#DivSaveInformationalRoles').show();
            $('#DivSaveContactsRoles').show();
            $('#DivSaveCompanyRoles').show();
            $('#DivSaveCommitteeRoles').show();
            $('#DivSaveTaskGroupRoles').show();
        }
        $('#hdnchkRadioId').val(myRadio.value);
    }
    else {
        if (self.gId() > 0 && self.gId() != null) {
            if (myRadio.value != "1") {
                $("input[name='EmailAdmin']").prop("disabled", true);
                $('#DivSaveInformationalRoles').hide();
                $('#DivSaveContactsRoles').hide();
                $('#DivSaveCompanyRoles').hide();
                $('#DivSaveCommitteeRoles').hide();
                $('#DivSaveTaskGroupRoles').hide();
            }
            else {
                $("input[name='EmailAdmin']").prop("disabled", false);
                $('#DivSaveInformationalRoles').show();
                $('#DivSaveContactsRoles').show();
                $('#DivSaveCompanyRoles').show();
                $('#DivSaveCommitteeRoles').show();
                $('#DivSaveTaskGroupRoles').show();
            }
            $('#hdnchkRadioId').val(myRadio.value);
        }
    }
}

function saveA4ARoles() {
    saveA4ACompanyNamePrimaryRoles();
    saveA4ACompanyNameAlternateRoles();
    saveA4AStaffRoles();
    saveA4AGroupUserRoles();
    $('#DivMsgA4AStaffContainer').hide();
    $('#DivCompanyMsg').show();
    $('#DivCommitteeMsg').hide();
    $('#DivInformationalContactMsg').hide();
    $('#DivSaveInformationalContactMsg').hide();
    $('#DivContactMsg').hide();
    $('#DivSaveContactMsg').hide();
    $('#DivTaskGroupMsg').hide();
    if ($('#hdnSaveRoleValues').val() == "1") {
        $('.spnMessage').html("Council/Committee Contact changes saved successfully").css("color", "green");
    }
    else if ($('#hdnSaveRoleValues').val() == "0") {
        $('.spnMessage').text("Error updating the records:");
    }
    else {
        $('.spnMessage').html("Council/Committee Contact changes saved successfully").css("color", "green");
    }
    setTimeout(function () { $('.spnMessage').text(""); }, 10000);
}

function saveA4ACompanyNamePrimaryRoles() {
    for (var key of CompanyNamePrimary.keys()) {
        A4AModelCompanyNamePrimary.push({
            groupId: self.gId(),
            CompanyName: key,
            value: CompanyNamePrimary.get(key),
            RoleId: "1",
        })
    }

    let dataobjPrimary = JSON.stringify(A4AModelCompanyNamePrimary)
    $.ajax({
        url: '/api/SaveCouncilCommitteeCompanyDtl',
        type: "post",
        data: dataobjPrimary,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('#hdnSaveRoleValues').val("1");
        },
        error: function (exception) {
            $('#hdnSaveRoleValues').val("0");
            msg = exception.responseText;
        }
    });

    CompanyNamePrimary = new Map();
    A4AModelCompanyNamePrimary = new Array();
}

function saveA4ACompanyNameAlternateRoles() {
    for (var key of CompanyNameAlternate.keys()) {
        A4AModelCompanyNameAlternate.push({
            groupId: self.gId(),
            CompanyName: key,
            value: CompanyNameAlternate.get(key),
            RoleId: "2",
        })
    }

    let dataobjAlternate = JSON.stringify(A4AModelCompanyNameAlternate)
    $.ajax({
        url: '/api/SaveCouncilCommitteeCompanyDtl',
        type: "post",
        data: dataobjAlternate,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('#hdnSaveRoleValues').val("1");
        },
        error: function (exception) {
            $('#hdnSaveRoleValues').val("0");
            msg = exception.responseText;
        }
    });

    CompanyNameAlternate = new Map();
    A4AModelCompanyNameAlternate = new Array();
}

function saveA4AStaffRoles() {
    for (var key of ChkStaff.keys()) {
        A4AModelChkStaff.push({
            groupId: self.gId(),
            UserId: $('#hdnChkStaffUserId').val(),
            value: ChkStaff.get(key),
            RoleId: "3",
        })
    }

    let dataobj = JSON.stringify(A4AModelChkStaff)
    $.ajax({
        url: '/api/SaveCouncilCommitteeStaffDtl',
        type: "post",
        data: dataobj,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('#hdnSaveRoleValues').val("1");
        },
        error: function (exception) {
            $('#hdnSaveRoleValues').val("0");
            msg = exception.responseText;
        }
    });

    ChkStaff = new Map();
    A4AModelChkStaff = new Array();
}

function saveA4AGroupUserRoles() {
    for (var key of ChkGroupUser.keys()) {
        A4AModelChkGroupUser.push({
            groupId: self.gId(),
            UserId: $('#hdnChkGroupUserId').val(),
            value: ChkGroupUser.get(key),
            RoleId: "4",
        })
    }

    let dataobj = JSON.stringify(A4AModelChkGroupUser)
    $.ajax({
        url: '/api/SaveCouncilCommitteeStaffDtl',
        type: "post",
        data: dataobj,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('#hdnSaveRoleValues').val("1");
        },
        error: function (exception) {
            $('#hdnSaveRoleValues').val("0");
            msg = exception.responseText;
        }
    });

    ChkGroupUser = new Map();
    A4AModelChkGroupUser = new Array();
}

function EmailAdmin(e) {
    SendEmailAdmin.set(e.value, e.checked)
}

function saveA4ACommitteeRoles() {
    for (var key of SendEmailAdmin.keys()) {
        A4AModelEmailAdmin.push({
            groupId: self.gId(),
            UserId: key,
            value: SendEmailAdmin.get(key)
        })
    }

    $('#DivMsgA4AStaffContainer').hide();
    $('#DivCompanyMsg').hide();
    $('#DivCommitteeMsg').show();
    $('#DivInformationalContactMsg').hide();
    $('#DivSaveInformationalContactMsg').hide();
    $('#DivContactMsg').hide();
    $('#DivSaveContactMsg').hide();
    $('#DivTaskGroupMsg').hide();
    let dataobj = JSON.stringify(A4AModelEmailAdmin)
    $.ajax({
        url: '/api/savecommitteeroles',
        type: "post",
        data: dataobj,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('.spnMessage').html("Council/Committee Contact changes saved successfully").css("color", "green");
        },
        error: function (exception) {
            $('.spnMessage').text("Error updating the records:" + exception.responseText);
        }
    });
    SendEmailAdmin = new Map();
    A4AModelEmailAdmin = new Array();
    setTimeout(function () { $('.spnMessage').text(""); }, 10000);
}

function chkInformationalRoles(e) {
    chkInformationalUser.set(e.value, e.checked)
}

function saveA4AInformationalRoles() {
    for (var key of chkInformationalUser.keys()) {
        A4AModelInformationalUser.push({
            groupId: self.gId(),
            UserId: key,
            value: chkInformationalUser.get(key)
        })
    }

    $('#DivMsgA4AStaffContainer').hide();
    $('#DivCompanyMsg').hide();
    $('#DivCommitteeMsg').hide();
    $('#DivInformationalContactMsg').hide();
    $('#DivSaveInformationalContactMsg').show();
    $('#DivContactMsg').hide();
    $('#DivSaveContactMsg').hide();
    $('#DivTaskGroupMsg').hide();
    let dataobj = JSON.stringify(A4AModelInformationalUser)
    $.ajax({
        url: '/api/savecommitteeroles',
        type: "post",
        data: dataobj,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('.spnMessage').html("Council/Committee Contact changes saved successfully").css("color", "green");
        },
        error: function (exception) {
            $('.spnMessage').text("Error updating the records:" + exception.responseText);
        }
    });
    chkInformationalUser = new Map();
    A4AModelInformationalUser = new Array();
    setTimeout(function () { $('.spnMessage').text(""); }, 10000);
}

function chkContactRoles(e) {
    chkContactUser.set(e.value, e.checked)
}

function saveA4AContactsRoles() {
    for (var key of chkContactUser.keys()) {
        A4AModelContactUser.push({
            groupId: self.gId(),
            UserId: key,
            value: chkContactUser.get(key)
        })
    }

    $('#DivMsgA4AStaffContainer').hide();
    $('#DivCompanyMsg').hide();
    $('#DivCommitteeMsg').hide();
    $('#DivInformationalContactMsg').hide();
    $('#DivSaveInformationalContactMsg').hide();
    $('#DivContactMsg').hide();
    $('#DivSaveContactMsg').show();
    $('#DivTaskGroupMsg').hide();
    let dataobj = JSON.stringify(A4AModelContactUser)
    $.ajax({
        url: '/api/savecommitteeroles',
        type: "post",
        data: dataobj,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('.spnMessage').html("Council/Committee Contact changes saved successfully").css("color", "green");
        },
        error: function (exception) {
            $('.spnMessage').text("Error updating the records:" + exception.responseText);
        }
    });
    chkContactUser = new Map();
    A4AModelContactUser = new Array();
    setTimeout(function () { $('.spnMessage').text(""); }, 10000);
}

function chkTaskGroupRoles(e) {
    chkTaskGroupUser.set(e.value, e.checked)
}

function saveA4ATaskGroupRoles() {
    for (var key of chkTaskGroupUser.keys()) {
        A4AModelTaskGroupUser.push({
            groupId: self.gId(),
            UserId: key,
            value: chkTaskGroupUser.get(key)
        })
    }

    $('#DivMsgA4AStaffContainer').hide();
    $('#DivCompanyMsg').hide();
    $('#DivCommitteeMsg').hide();
    $('#DivInformationalContactMsg').hide();
    $('#DivSaveInformationalContactMsg').hide();
    $('#DivContactMsg').hide();
    $('#DivSaveContactMsg').hide();
    $('#DivTaskGroupMsg').show();
    let dataobj = JSON.stringify(A4AModelTaskGroupUser)
    $.ajax({
        url: '/api/savecommitteeroles',
        type: "post",
        data: dataobj,
        contentType: 'application/json',
        dataType: "Json",
        success: function (result) {
            $('.spnMessage').html("Task Force changes saved successfully").css("color", "green");
        },
        error: function (exception) {
            $('.spnMessage').text("Error updating the records:" + exception.responseText);
        }
    });
    chkTaskGroupUser = new Map();
    A4AModelTaskGroupUser = new Array();
    setTimeout(function () { $('.spnMessage').text(""); }, 10000);
}