﻿/// <reference path="../Scripts/knockout-3.1.0.js" />
/// <reference path="../Scripts/jquery-1.10.2.js" />


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
                    } } },
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
                    url: '/api/group?isEdit='+self.edit(),
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
                    if ($(".GroupTitle").html().indexOf("Group:") < 1) {
                        $(".GroupTitle").append("<br/> <a  href='/group/show/?gid=" + data.GroupId + "' class='spnGroupName niceLabel-black   margin-topbot10'>Group: " + $('#hdnGroupName').val() + "</a");
                    }
                    $(".NoEdit").prop("disabled", true).css("background-color", "lightgrey");
                    groupRegisterViewModel.addA4AStaffViewModel.gId(data.GroupId);
                    groupRegisterViewModel.UMSubscribeModel.gId(data.GroupId);
                    groupRegisterViewModel.addTaskForceWorkGroup.gId(data.GroupId);
                    councilCommitteeViewModel.gId = data.GroupId;
                    groupRegisterViewModel.addA4AStaffViewModel.hasBounceReports(data.BounceReports);
                });
            }
        
    };
    self.load = function (grp) {
        try
        {
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
            self.LyrisSendId(ATAgroup.LyrisSendId);
            self.DepartmentId(ATAgroup.DepartmentId);
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
        catch (err)
        {
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
            error: function (exception) {}
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
                } else {
                    self.ErrorMessage("User" + newValue[key].name + " already exists!");
                    setTimeout(function () { self.ErrorMessage(""); }, 5000);
                }
            }
        }
        else {
            self.ErrorMessage("No Group ID exists!");
            setTimeout(function () { self.ErrorMessage(""); }, 5000);
        }
        
    });
    self.removeA4AStaff = function (staff) {
        $('.overlay').show();
        var gtypeid = $('#hdnGroupTypeId').val();
        var url = '/api/RemoveGroupUser?GroupTypeId=' + gtypeid + '&IsA4AStaff=true'; 
        var dataObject = ko.utils.stringifyJson(staff);
        dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
        $.ajax({
            url: url,
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                self.A4AStaff.remove(staff);
                self.InfoMessage("A4A staff removed successfully!");
                setTimeout(function () { self.InfoMessage(""); },5000);
            },
            error: function (xhr, status, error) {
             $(".overlay").hide(); 
                self.ErrorMessage("Error:" + xhr.responseText);
                setTimeout(function () { self.ErrorMessage(""); }, 10000);

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
                setTimeout(function () {                    
                    self.InfoMessage(""); 
                }, 5000);
            },
            error: function (xhr, status, error) { 
                self.ErrorMessage("Error:" + xhr.responseText);
                $(".overlay").hide();
                setTimeout(function () { self.ErrorMessage(""); },10000);
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
            try
            { 
                //$('#acSubInfoUser').attr("value", "");
                //$("#acSubInfoUser").val('');
                // $(".ui-autocomplete-input").val("");
                $(".acSubInfoUser").val("");
            }
            catch( exception)
            {
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
                            label:value , //exchanged by NA - changed the reuslt to sorted dictionary 3/26/2019
                            text:key,
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
            try
            {
            $(".overlay").show();
            //Get Gid now 
            var gId = 0;
            gId = $('#hdnGroupId').val(); ///check for groupid value.TODO:cleaning up the code 
            if (!(gId > 0) && !(ViewModel.prototype.gId() > 0)) {
                ViewModel.prototype.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
                setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 5000);
            }
            else {
                ViewModel.prototype.errorMessage("");
            }
            var role = '';
            var temp,temp1;
                

            //Get CompanyName out of the username
            var UsernCompname = ui.item.label;
            var startindex = UsernCompname.indexOf('(');
            var cname = UsernCompname.substring(startindex + 1).replace(')', ''); //Get the Company out of the Company Name string.
            var usrname = UsernCompname.replace(UsernCompname.substring(startindex), ''); //Get the companyname out of the username
                 
            if (+$("#hdnSubscribeInformational").val() === 1) //ViewModel.prototype.SorI not working for somereason so changed to hidden field.
            {
                role = "Participant";
                temp = [{ "Name": UsernCompname, "UserId": ui.item.text, "Participant": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname }];
                temp1 = { "Name": UsernCompname, "UserId": ui.item.text, "Participant": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname };
            }
            else
                if (+$("#hdnSubscribeInformational").val() === 2) {
                role = "Informational";
                temp = [{ "Name": UsernCompname, "UserId": ui.item.text, "Informational": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname }];
                temp1 = { "Name": UsernCompname, "UserId": ui.item.text, "Informational": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": cname };
            }
            else
            {
               ViewModel.prototype.errorMessage("Subscription group type is not selected, Contact IT for help!");
               setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 5000);
               return;
            }
                
            var dataObject = ko.utils.stringifyJson(temp);            
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                    ViewModel.prototype.selectedValues.push(temp1);
                    ViewModel.prototype.InfoMessage("User " + usrname + " has been successfully added!");
                    setTimeout(function () { ViewModel.prototype.InfoMessage(""); }, 5000);
                },
                error: function (exception) {
                    try
                    { 
                        ViewModel.prototype.errorMessage("Error in adding user to the group:" + exception.responseText); 
                        setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 4000);
                    }
                    catch (Err)
                    { 
                        alert("Error in adding user to group:" + exception.responseText);
                    } 
                }
            }).done(function (data) {}); 
            }
            catch (Err)
            { 
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
            try
            {
            $(".overlay").show();
            //Get Gid now
            var gId = 0; 
            ViewModel.prototype.errorMessage = ko.observable(); //Check on this
            if (isNaN(ViewModel.prototype.gId()))
            { gId = $('#hdnGroupId').val(); }
            else {
                gId = ViewModel.prototype.gId();
            }
            var company = '';
            if (ui.item.label) { 
                company = ui.item.label.trim().slice(ui.item.label.indexOf('(') + 1, -1); 
                $('#hdnChairComp').val(company); 
                var txtprim = $("#Prim" + company.replace(/\s/g, '')); 
                //alert("company" + company + " txtprim:" + txtprim.val());
                try
                {
                    if ((txtprim !== null) && txtprim.val().length > 0) {
                        $('.spnMessage').text("You already have a primary representative for " + company + ".  Select another company chair or delete the contact below.").css("color", "red");
                        setTimeout(function () { $('.spnMessage').html(""); }, 5000);
                        $("#GroupChairGGA").val(""); //Clear the GGA Textbox
                        $('#hdnChairComp').val(""); //Clear the company hidden field too
                        $(".overlay").hide();
                        return false;
                    }
                    else { 
                        $("#Prim" + company.replace(/\s/g, '')).attr("disabled", "disabled"); 
                    }
                }
                catch (Err)
                {
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
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) { 
                    $('.spnMessage').html(ui.item.label + "has been successfully saved as Chair!!").css("color", "green"); 
                    setTimeout(function () { $('.spnMessage').html(""); }, 5000);
                },
                error: function (exception) {
                    $('.spnMessage').html("Error in saving Group Chair" + exception.responseText).css("color", "red");  
                    $("#GroupChairGGA").val("");  //Clear the Group chair textbox as the chair is not saved
                    $('#hdnChairComp').val(""); //Clear the company hidden field too so Primary can be added
                    setTimeout(function () { $('.spnMessage').html(""); }, 5000);
                }
            }).done(function (data) { 
               
            }); 
            }
            catch (Err)
            {
                $(".overlay").hide();
            }
            $(".overlay").hide();
        };
        /////////////////////////////////////////////////////////////////////////////////
        ////////////Vice CHAIR SAVING
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectViceChair = function (event, ui) {
            try 
            { 
             $(".overlay").show();  
            //Get Gid now
            var gId = 0;
            if (isNaN(ViewModel.prototype.gId()))
            { gId = $('#hdnGroupId').val(); }
            else
                gId = ViewModel.prototype.gId();
            ViewModel.prototype.errorMessage = ko.observable();
            var company = '';
            if (ui.item.label) {
                company = ui.item.label.trim().slice(ui.item.label.indexOf('(') + 1, -1);
                $('#hdnViceChairComp').val(company);
                try
                {
                var txtprim = $("#Prim" + company.replace(/\s/g, '')); 
                if ((txtprim !== null) && txtprim.val().length > 0) {
                    $('.spnMessage').html("You already have a primary representative for " + company + ".  Select another company vice chair or delete the contact below.").css("color", "red");
                    setTimeout(function () { $('.spnMessage').html(""); }, 5000); 
                    $("#GroupViceChairGGA").val(""); //Clear the textbox 
                    $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                    $(".overlay").hide();
                    return false;
                }
                else { 
                    $("#Prim" + company.replace(/\s/g, '')).attr("disabled", "disabled");
                } 
                }
                catch (Err) { 
                }
            }
            if (!(company.length > 0))
            { 
                $('.spnMessage').html("No company name found for Vice Chair. So the selection could not be completed");
                setTimeout(function () { $('.spnMessage').html(""); }, 5000);
                return;
            }
            var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "ViceChair": true, "GroupId": ViewModel.prototype.gId(), "CompanyName": company }];
            var dataObject = ko.utils.stringifyJson(temp);
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) { 
                    $('.spnMessage').html(ui.item.label + "has been successfully saved as Vice Chair!!").css("color", "green"); 
                    setTimeout(function () { $('.spnMessage').html(""); }, 5000);
                    $('#hdnPrimary').val();
                },
                error: function (exception) { 
                    $('.spnMessage').html("Error in saving Group Vice Chair" + exception.responseText).css("color", "red");
                    $("#GroupViceChairGGA").val(""); //clear the textbox 
                    $('#hdnViceChairComp').val(""); //Clear the company hidden field too
                    setTimeout(function () { $('.spnMessage').html(""); }, 5000);
                }
            }).done(function (data) {
            });
            }
            catch (Err)
            { $(".overlay").hide(); }
            $(".overlay").hide();
        };
        /////////////////////////////////////////////////////////////////////////////
        /***   Participant/Informational - Add all users to Group - not being used ***/
        ////////////////////////////////////////////////////////////////////////////
        ViewModel.prototype.addGroup = function () {
            try
            {
                $(".overlay").show();
            var dataObject = ko.utils.stringifyJson(ViewModel.prototype.selectedValues);
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
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
            catch (Err)
            { $(".overlay").hide(); }
            $(".overlay").hide();
        };
        //////////////////////////////////////////////////////////////////////////////
        /***   Participant/Informational - Delete User from UserGroup in the DB     ***/
        //////////////////////////////////////////////////////////////////////////////
        ViewModel.prototype.removeUMContacts = function (contact) {
            try
            { 
             $(".overlay").show();  

             var gtypeid = $('#hdnGroupTypeId').val();  
             var dataObject = ko.utils.stringifyJson(contact); 
             dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
              
             $.ajax({
                url: '/api/RemoveGroupUser?IsA4AStaff=0&GroupTypeId=' + gtypeid,
                type: 'post',
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
            catch (Err)
            { $(".overlay").hide(); }
            setTimeout(function () { $(".overlay").hide(); }, 2000);
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

                if (isNaN(ViewModel.prototype.gId()))
                { gId = $('#hdnGroupId').val(); }
                else
                    gId = ViewModel.prototype.gId();
                var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 3
                //Delete the user from database and clear the field


                ViewModel.prototype.RemoveGroupUser(URL, '', 'Chair');
                $(event.target).prev().val('');
            }      catch (Err)
            { $(".overlay").hide(); }
            $(".overlay").hide();
        }
        ViewModel.prototype.removeViceChairGroupUser = function (item, event) {
            try {
                $(".overlay").show();
            /* Note: Reset the hidden value */
            $('#hdnViceChairComp').val(''); 

            if (isNaN(ViewModel.prototype.gId()))
            { gId = $('#hdnGroupId').val(); }
            else
                gId = ViewModel.prototype.gId();
            var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 4;
            /* Note: Delete the user from database and clear the field  */
            ViewModel.prototype.RemoveGroupUser(URL,'','Vice Chair');
            
            $(event.target).prev().val('');
        }      catch (Err)
        { $(".overlay").hide(); }
            $(".overlay").hide();
        }
        ViewModel.prototype.DisablePrimBox = function () {
            try
            {
            /* Note: Enabling Prim text boxes because the Chair/ViceChair user has been removed **/
            $("[id*=Prim]").prop("disabled", false);
            /* Note:  Disabling just the chair and vice chair company */
            $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled"); 
            $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
            }
            catch (err)
            {
              // TODO: Thinkhow to handle this  $('.spnMessage').append("Error disabling the Primary!!");
            }
        };
        ViewModel.prototype.RemoveGroupUser = function (URL) {
          
            $.ajax({
                url: URL,
                type: 'post',
                contentType: 'application/json',
                success: function (data) {
                    ViewModel.prototype.DisablePrimBox();
                    $('.spnMessage').text("User removed successfully!!").css("color", "green");
                    setTimeout(function(){ $('.spnMessage').text(""); }, 5000);
                     
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")"); 
                    $('.spnMessage').text("Error removing user!:" + xhr.responseText).css("color", "red"); 
                    setTimeout(function () { $('.spnMessage').text(""); }, 5000);
                    $(event.target).parent().append("<span id='" + compPrimError + "' class='text-danger'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message + "</span>");
                  
                    return false;
                }
            })
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
        { id: "3", name: "Chair"},
        { id: "15", name: "Co Chair"},
        { id: "16", name: "FAA Chair"},
        { id: "17", name: "FAA Co Chair"},
        { id: "19", name: "Observer"}, 
        { id: "8", name: "Secretary"},
        { id: "6", name: "Spokesperson"}, 
        { id: "12", name: "Participant"},
        { id: "18", name: "Team Leader"},  
        { id: "4", name: "Vice Chair"}]); 
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
                            text:key ,
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
                                setTimeout(function () { $("#tskErrorMsg").html("")},5000);
                                ViewModel.prototype.selectedTskForUser.push(temp);
                                $(event.target).val(''); //Note: Clears the textbox after the user is added 
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                $("#tskErrorMsg").css("color", "red");
                                $("#tskErrorMsg").html("Error adding user to group:" + jqXHR.responseText); // TODO: Fix it to use ViewModel.prototype.errorMessage. For somereason ViewModel.prototype.errorMessage was not updating and showing up 1/18/18
                                setTimeout(function () { $("#tskErrorMsg").html("") }, 5000);
                            }
                        });
                    }
                }        
                else {
                    $("#tskErrorMsg").css("color", "red");
                    ViewModel.prototype.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
                }
            }    
        
          catch (Err)
          { $(".overlay").hide(); }
            $(".overlay").hide();
        
  };
        /////////////////////////////////////////////////////////////////////
        /***   taskforce- Delete User from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.removeUMContacts = function (contact) {
            try
            {
          $(".overlay").show();
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
                    setTimeout(function () { ViewModel.prototype.errorMessage() },5000);
                    
                },
                error: function (xhr, status, error) {
                    $("#tskErrorMsg").css("color", "red");
                    ViewModel.prototype.errorMessage("Error in removing" + contact.Name + xhr.responseText); 
                    setTimeout(function () { ViewModel.prototype.errorMessage() }, 5000);
                    $(".overlay").hide();
                    return false;
                }
            });
            ViewModel.prototype.selectedTskForUser.remove(contact);
            }
            catch (Err)
            {
                ViewModel.prototype.errorMessage("Error in removing" + contact.Name + Err);
                $(".overlay").hide();
            }
            $(".overlay").hide();
        };
        /////////////////////////////////////////////////////////////////////
        /***   taskforce- LOAD Users from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.load = function (groupid) {
            //("in tsk load");
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
                        temp['CompanyName'] =  result[i].CompanyName ; //Removed (  )  because we made company seperate field
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
        try
        {
          $(".overlay").show()
        if ($('#hdnChairComp').val().trim() === params.Company.trim())
        {
            $('.spnMessage').html(params.Company + " representative has been set as Chair. So cannot add Primary/Alternate contact for them").css("color", "red");
            setTimeout(function () { $('.spnMessage').html(""); }, 5000);
            $(".overlay").hide();
            return false;
        }
        if ($('#hdnViceChairComp').val().trim() === params.Company.trim()) {
            $('.spnMessage').html(params.Company + " representative has been set as Vice Chair. So cannot add Primary/Alternate contact").css("color", "red");
            setTimeout(function () { $('.spnMessage').html(""); }, 5000);
             $(".overlay").hide();
             return false;
        }

        var compPrimError = (params.Company).replace(/\s+/g, '') + "PrimError";
        /**Remove Error Message*/
        var primErrItem = $('#' + compPrimError);
        if (typeof primErrItem !== "undefined")
            primErrItem.remove();

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
                setTimeout(function () { $('.spnMessage').html(""); }, 5000);
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                var par = $(event.target).parent();
                par.append("<span id='" + compPrimError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message + "</span>");
                $(event.target).prev().val('');
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
        var URL = "/api/RemoveGrpUsrbyComp/?CompanyName=" + params.Company + "&GroupId=" + gId + "&RoleId=" + 1;
        //Delete the user from database and clear the field
        RemoveGroupUser(URL, params.Company,"Primary");
        $(event.target).prev().val('');
    }
    self.removeAltGroupUser = function (item, event) {
        var URL = "/api/RemoveGrpUsrbyComp/?CompanyName=" + params.Company + "&GroupId=" + gId + "&RoleId=" + 2;
        //Delete the user from database and clear the field
        RemoveGroupUser(URL, params.Company,"Alternate");
        $(event.target).prev().val('');
    }
    function RemoveGroupUser(URL,Company,Role) {
        $.ajax({
            url: URL,
            type: 'post',
            contentType: 'application/json',
            success: function (data) { 
                $('.spnMessage').html("Removed " + Role + " for " + Company).css("color", "green");
                setTimeout(function () { $('.spnMessage').html(""); }, 5000);
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")"); 
                $(event.target).parent().append("<span id='" + compPrimError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message + "</span>");
                $(".overlay").hide();
                return false;
            }
        })
    }
    self.selectAlternate = function (event, ui) {
        try
        {
          $(".overlay").show();
        var compAltError = (params.Company).replace(/\s+/g, '') + "PrimAlt"; 
        /**Remove Error Message*/
        var altErrItem = $('#' + compAltError);
        if (typeof altErrItem !== "undefined")
            altErrItem.remove();

        //Get GroupId 
        if(!(gId > 0))
        gId = $('#hdnGroupId').val() || parseInt(getGID('gid'));

        var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Alternate": true, GroupId: gId, CompanyName: params.Company }];
        var dataObject = ko.utils.stringifyJson(temp);
        
        $.ajax({
            url: '/api/UserGroup',
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                 $('.spnMessage').html(ui.item.label + " has been successfully added as Alternate for " + params.Company).css("color", "green");
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                $(event.target).parent().append("<span id='" + compAltError + "' class='error'><br/><i class='fa fa-times-circle'></i>" + err.Message + "</span>");
                $(event.target).prev().val('');
                $(".overlay").hide();
                return false;
            }
        }) 
        }
        catch (Err) {
        $(".overlay").hide();
        } $(".overlay").hide();
    };
}
/// END OF Council/Committee


/////////////////////////////////////////////////////////////////////////////
// BEgin Council/Committee GGA
///////////////////////////////////////////////////////////

///////////////////////////////////////
//GGA CommitteePrimAlt
////////////////////////////////////// 
var CommitteePrimAlt = function (Name, UserId, Type, CompanyName, Alternate, Primary) {
    this.Name = ko.observable(Name);
    this.UserId = ko.observable(UserId);
    this.CompanyName = ko.observable(CompanyName);
    this.Type = ko.observable(Type);
    this.Alternate = ko.observable(Alternate);
    this.Primary = ko.observable(Primary);
}

var councilCommitteeGGAViewModel = function ()
{
    var self = this;
    self.gId = ko.observable($('#hdnGroupId').val()); 
    var gId = $('#hdnGroupId').val();
    self.errorMessage = ko.observable();
    self.infoMessage = ko.observable();
    self.people = ko.observableArray([]).distinct('CompanyName');
    self.Roles = ko.observableArray([
        { id: "2", name: "Alternate" },
        { id: "1", name: "Primary" }]);
    self.role = ko.observable();
    self.Companies = ko.observableArray();
    self.company = ko.observable();
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
    this.selectPrimAltUser4Company = function (event, ui) {   
        if (!isNaN($('#hdnGroupId').val())) {
            self.gId($('#hdnGroupId').val());
            gId = $('#hdnGroupId').val();
        }
        if ((isNaN(gId) || (isNaN(self.gId())))) {
            self.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
            setTimeout(function () { self.errorMessage(""); }, 5000);
        }
        else { 
            var role = self.role(); 
            var roleName = $("#inpGGAGroupRole option[value='" + role + "']").text();
            var comp = self.company();
            var CompanyName = $("#inpGGACompanies option[value='" + comp + "']").text();

            //Get the Primary & Alternate companies so there is no primary selected for that company  
            if (typeof role === 'undefined' || typeof comp === 'undefined') {
                self.errorMessage("Role or CompanyName is Missing. The user cannot be added to the group.");
                setTimeout(function () { self.errorMessage(""); }, 5000);
            }
            else
                if (roleName.toLowerCase().trim().indexOf('primary') !== -1 && ($('#hdnChairComp').val().trim().indexOf(CompanyName) !== -1 || $('#hdnViceChairComp').val().trim().indexOf(CompanyName) !== -1)) {
                    //alert($('#hdnChairComp').val() + "  " + $('#hdnViceChairComp').val());
                    self.errorMessage("This company already has a representative assigned for Chair/ViceChair. So cannot add Primary!"); //Removed the alternate and added clause check only for adding primary users if a chair/vice chair is selected
                    setTimeout(function () { self.errorMessage(""); }, 5000);
                }
                else
                {
                    //Set the item to push to DB & knockout array
                    var temp = {};
                    temp['Name'] = ui.item.label;
                    temp['UserId'] = ui.item.text;
                    temp['Type'] = roleName;
                    temp[roleName.replace(/\s/g, '').trim()] = true;
                    temp['GroupId'] = self.gId();
                    temp['CompanyName'] = CompanyName;
                    var dataObject = ko.utils.stringifyJson(temp); 
                    dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
                  
                    //First check if the user already exists in the array
                    var match = ko.utils.arrayFirst(self.people(),
                        function (item) {
                            return ui.item.text == item.UserId();
                        });
                    if (!match) { 
                         //Now check if its a primary and some primary already exists for the company
                        if (roleName === "Primary") {
                            var PrimaryExists = false;
                            var k = $.getJSON('/api/checkifcompanyhasrole?GroupId=' + self.gId() + "&CompanyName=" + CompanyName + "&RoleId=" + 1, function (data) { 
                                PrimaryExists = data
                            }).done(
                                function () {
                                    if (!PrimaryExists) {
                                        $.ajax({
                                            url: '/api/UserGroupGGA',
                                            type: 'post',
                                            data: dataObject,
                                            contentType: 'application/json',
                                            success: function (data) {
                                                //Values to push to Json when Role Type is Primary
                                                self.people.push(new CommitteePrimAlt(ui.item.label, ui.item.text, roleName, CompanyName, true, false));
                                                self.infoMessage("Successfully added a Primary user to " + CompanyName);
                                                setTimeout(function () { self.infoMessage(""); }, 4000);
                                                //Reset values
                                                $("#ggaUser").val("");
                                                self.role(null);
                                                self.company(null);
                                            },
                                            error: function (xhr, status, error) {
                                                self.errorMessage("Error in adding user: " + xhr.responseText);
                                                setTimeout(function () { self.errorMessage(""); }, 6000);
                                            }
                                        });
                                    }
                                    else {
                                        self.errorMessage("A primary has already been set for this " + CompanyName+".Please delete the current primary and then add this user!");
                                        setTimeout(function () { self.errorMessage(""); }, 6000);
                                        //Reset values
                                        $("#ggaUser").val("");
                                        self.role(null);
                                        self.company(null);

                                    }
                                });
                        }//End of IsPrimaryRole
                        else {
                            //Just add Alternates to the group
                            $.ajax({
                                url: '/api/UserGroupGGA',
                                type: 'post',
                                data: dataObject,
                                contentType: 'application/json',
                                success: function (data) { 
                                    //Values to push to Json when Role Type is Alternate
                                    self.people.push(new CommitteePrimAlt(ui.item.label, ui.item.text, roleName, CompanyName, false, true)); 
                                    self.infoMessage("Successfully added an Alternate user to " + CompanyName); 
                                    setTimeout(function () { self.infoMessage(""); }, 4000);
                                    //Reset values
                                    $("#ggaUser").val("");
                                    self.role(null);
                                    self.company(null);
                                },
                                error: function (xhr, status, error) {
                                    self.errorMessage("Error in adding user: " + xhr.responseText);
                                    setTimeout(function () { self.errorMessage(""); }, 6000);
                                }
                            });
                        }
                    }
                 else {
                    self.errorMessage("The user " + match.Name() + " already exists under " + match.CompanyName() + " ");
                    setTimeout(function () { self.errorMessage(""); }, 5000);
                }
                $("#ggaUser").val('');
              }            
        }
        };

        
     
    self.removeCommitteePrimAlt = function (CommitteePrimAlt) { 
        if (isNaN(self.gId())) {
            self.errorMessage("GroupId is missing. The user cannot be removed from the group.");
            setTimeout(function () { self.errorMessage(""); }, 5000);
            return false;
        }
        if (isNaN(CommitteePrimAlt.UserId())) {
            self.errorMessage("User Information is missing. The user cannot be removed from the group. Please contact IT");
            setTimeout(function () { self.errorMessage(""); }, 5000);
            return false;
        }
        else {
            self.people.remove(CommitteePrimAlt);
            var role = 1;
            if (CommitteePrimAlt.Type() === "Alternate")
                role = 2;
            //var URL = "/api/RemoveGrpUsrbyComp/?CompanyName=" + CommitteePrimAlt.CompanyName() + "&GroupId=" + self.gId() + "&RoleId=" + role;
            var URL = "/api/RemoveGrpUsrbyUserId/?UserId=" + CommitteePrimAlt.UserId() + "&GroupId=" + self.gId() + "&RoleId=" + role;
            if (RemoveGroupUser(URL, CommitteePrimAlt.CompanyName, CommitteePrimAlt.Type)) { 
                if (role === 1) {
                    $("#Prim" + company.replace(/\s/g, '')).val("");
                   // alert("Resetting the alert!" + $("#Prim" + company.replace(/\s/g, '')).val());
                }
            }
            return true;
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
                setTimeout(function () { self.infoMessage(""); }, 5000);
                return true;
            },
            error: function (xhr, status, error) {
                self.errorMessage("Error occured in remove group user!" + xhr.responseText);
                setTimeout(function () { self.errorMessage(""); }, 5000);
                return false;
            }
        })      
    }

    //Added by NA - 9/20/2018 on JDs request to have a clear button clear the textboxes 
    this.clearGGAUser = function () {
        $("#ggaUser").val(""); 
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
                        self.people.push(new CommitteePrimAlt(result[i].UserName, result[i].UserId, result[i].Role, result[i].CompanyName, true, false));
                    } else
                        if (result[i].RoleId === 2) {
                            self.people.push(new CommitteePrimAlt(result[i].UserName, result[i].UserId, result[i].Role, result[i].CompanyName, false, true));
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
                setTimeout(function () { self.errorMessage(""); }, 5000);
            }
        });
    };
};
function DisablePrimary() {
    try {
        /* Note: Enabling Prim text boxes because the Chair/ViceChair user has been removed **/
        $("[id*=Prim]").prop("disabled", false);
        var obj = $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')); 
        /* Note:  Disabling just the chair and vice chair company */
        $("#Prim" + $('#hdnChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
        $("#Prim" + $('#hdnViceChairComp').val().replace(/\s/g, '')).attr("disabled", "disabled");
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
    $("#breadcrumb ul").append('<li><a href="/group/list/"> Group List </a></li>');

    // Register knockout UI components
    ko.components.register('primaryalternate', {
        viewModel: councilCommitteeViewModel,
        template: "<div class='row' style='margin-bottom:8px;' > <div class='col-sm-2'> <span class='niceLabel' data-bind='text: Company'></span></div>" +
        "<div class='col-sm-5' ><input type='text' class='P1 chosen form-control' style='width:300px;' data-bind='value:PrimUserValue,attr: { id:PrimaryID()},ko_autocomplete: { source: getCompanyUsers, select: selectPrimary ,minLength: 3,close: closeSelect }'/>" +
        "<a href='#' class='text-danger' style='vertical-align: middle;padding-left:7px;'  data-bind='click: $data.removePrimGroupUser'>X</a></div>" +
        "<div class='col-sm-5' ><input type='text' class='A1 chosen form-control' style='width:300px;'    data-bind='value:AltUserValue,attr: { id:AlternateID() },ko_autocomplete: { source: getCompanyUsers, select: selectAlternate ,minLength:3,close: closeSelect }'/>" +
        "<a href='#' class='text-danger' style='vertical-align: middle;padding-left:7px;'  data-bind='click: removeAltGroupUser'>X</a></div></div>"

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
        //alert("Got In SetPrimaryAlternate 1");
        var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid; 
        $.ajax({
            url: query,
            type: "GET",
            contentType: 'application/json',
            success: function (data) {
                var result = JSON.parse(data);
                //alert("In SetPrimaryAlternate ");
                for (i = 0; i < result.length; i++) {
                    try
                    {
                        var Name = result[i].UserName;
                        var t = result[i].CompanyName.replace(/\s/g, '');
                        if (result[i].RoleId === 1) {
                            sessionStorage.setItem("Prim" + t, Name);
                        }
                        else
                            if (result[i].RoleId === 2) {
                                sessionStorage.setItem("Alt" + t, Name);
                            }
                            else
                                if (result[i].RoleId === 3) { 
                                    $('#GroupChair').val(Name + ' (' + t + ')');
                                    $('#hdnChairComp').val(result[i].CompanyName); 
                                    //DisablePrimary();
                                }
                                else
                                    if (result[i].RoleId === 4) { 6
                                        $('#GroupViceChair').val(Name + ' (' + t + ')');
                                        $('#hdnViceChairComp').val(result[i].CompanyName);
                                        //DisablePrimary();
                                    }
                    }
                    catch(err)
                    {
                        console.log("error in setting primary/Alternate"+err);
                    }
                }
            },
            error: function (exception) { //Add Message //alert('Exception in Subscribe/Informational user list:' + exception.responseText); 
            }
        });
    }
    ko.validation.init({ 
        insertMessages: false
    }); 

    if (parseInt(getGID('gid')) > 0) {
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
