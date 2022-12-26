/// <reference path="../Scripts/knockout-3.1.0.js" />
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
    self.LyrisShortDescription = ko.observable(LyrisShortDescription);//.extend({ required: true }); //.extend({ required: "Please enter a Lyris Short Description" });
    self.GroupSiteUrl = ko.observable(GroupSiteUrl);
    self.ParentGroupId = ko.observable(ParentGroupId);
    self.Liaison1 = ko.observable(Liaison1);//.extend({ required: true });
    self.Mission = ko.observable(Mission);
    self.LyrisSendId = ko.observable(LyrisSendId);
    self.DepartmentId = ko.observable();
    self.rdoBounceReports = ko.observable("Off");
    self.InfoMessage = ko.observable();
    self.gId = ko.observable();
    self.edit = ko.observable(false);

    self.step1Validation = ko.validatedObservable([
    self.GroupName.extend({ required: { message: ' Group Name is required.' } }),
    self.LyrisListName.extend({ required: { message: ' List Name is required.' } }),
    self.Liaison1.extend({ required: { message: ' Please choose a Liaison.' } }),

    //self.ZipCode.extend({
    //validation: {
    //    validator: function (val) {
    //        return /^(\d{5})?$/.test(val);  ///^[0-9]{5}$/
    //    },
    //    message: 'ZipCode can only contain 5 digits!'
    //}
   //})
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
                        name: value.DepartmentDetail,         // Push the key on the array
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
        var result = ko.validation.group(groupRegisterViewModel.addGroupViewModel, { deep: true }); 
        $(".overlay").show();  
        if (!self.step1Validation.isValid()) {
            self.ShowErr(true);
            self.step1Validation.errors.showAllMessages(); 
            $(".overlay").hide();
            return false;
        }
        else 
        { 
            $('.overlay').show();
            self.ShowErr(false);
            var dataObject = ko.toJSON(this);
            $.ajax({
                url: '/api/group',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                },
                error: function (exception) {
                    $(".overlay").hide();
                    self.InfoMessage("Error Occured: " + exception.responseText);
                    setTimeout(function () { self.InfoMessage(""); }, 3000);

                }
            }).done(function (data) {
                $(".overlay").hide();
                self.InfoMessage("Group has been saved successfully");
                self.HiddenGroupId(data.GroupId); 
                $('#hdnGroupId').val(data.GroupId);
                $('#hdnGroupName').val(self.GroupName());
                $("#Headtitle").html("Create New Group"); 
                if ($(".GroupTitle").html().indexOf("Group:") < 1)
                {
                    $(".GroupTitle").append("<br/> <span   class='spnGroupName niceLabel-black   margin-topbot10'>Group: " + $('#hdnGroupName').val() + "</span");
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
        var ATAgroup = JSON.parse(grp.responseText);
        self.GroupName(ATAgroup.GroupName);
        self.AppliesToSiteId(ATAgroup.AppliesToSiteId);
        self.GroupTypeId(ATAgroup.GroupTypeId);
        self.LyrisListName(ATAgroup.LyrisListName);
        self.LyrisShortDescription(ATAgroup.LyrisShortDescription);
        self.GroupSiteUrl(ATAgroup.GroupSiteUrl);
        self.ParentGroupId(ATAgroup.ParentGroupId);
        self.Liaison1(ATAgroup.Liaison1UserId);
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
        //alert(self.edit());
        $(".NoEdit").prop("disabled", true).css("background-color", "lightgrey");
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
        // alert("Choose a option from dropdown to add user info!");
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
    }
    self.selected.subscribe(function (newValue) {
        if (!isNaN(self.gId())) {
            for (var key in newValue) {
                //Check if the user already exists
                var match = ko.utils.arrayFirst(self.A4AStaff(),
                       function (item) {
                           return newValue[key].value === item.UserId;
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
        var dataObject = ko.utils.stringifyJson(staff);
        dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
        $.ajax({
            url: '/api/RemoveGroupUser',
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
            },
            error: function (xhr, status, error) {
                self.ErrorMessage("Error:" + xhr.responseText);
                setTimeout(function () { self.ErrorMessage(""); }, 5000);
                return false;
            }
        });
        self.A4AStaff.remove(staff);
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
                setTimeout(function () { self.ErrorMessage(""); }, 5000);
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
                        GroupId: result[i].GroupId,//264,
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
                setTimeout(function () { self.InfoMessage(""); }, 5000);
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
        ViewModel.prototype.SorI = 0;
        ViewModel.prototype.getLanguages = function (request, response) { 
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
                            label: key,
                            text: value,
                            value: ""
                        }
                    }));
                }
            });
        };
        ViewModel.prototype.selectedValues = ko.observableArray();
        /////////////////////////////////////////////////////////////////////
        /***  Subscribe/Informational -  Select User to Add     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectLanguage = function (event, ui) {
            //Get Gid now 
            var gId = 0;
            gId = $('#hdnGroupId').val(); ///check for value..when cleaning up the code 
            if (!(gId > 0) && !(ViewModel.prototype.gId() > 0)) {
                ViewModel.prototype.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
                setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 5000);}
            else {
                ViewModel.prototype.errorMessage(""); 
            }
            var role = '';
            var temp;
            if (+$("#hdnSubscribeInformational").val() === 1) //ViewModel.prototype.SorI not working for somereason so changed to hidden field.
            {
                role = "Subscribe";
                temp = [{ "Name": ui.item.label, "UserId": ui.item.text, Subscribe: true, GroupId: ViewModel.prototype.gId(), CompanyName: '' }]; 
            }
            else
                if (+$("#hdnSubscribeInformational").val() === 2) {
                role = "Informational";
                temp = [{ "Name": ui.item.label, "UserId": ui.item.text, Informational: true, GroupId: ViewModel.prototype.gId(), CompanyName: '' }]; 
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
                    ViewModel.prototype.selectedValues.push({ "Name": ui.item.label, "UserId": ui.item.text, role: true, GroupId: ViewModel.prototype.gId(), CompanyName: '' });
                    ViewModel.prototype.InfoMessage("User " + ui.item.label + " has been successfully added successfully!");
                    setTimeout(function () { ViewModel.prototype.InfoMessage(""); }, 5000);
                },
                error: function (exception) {
                    ViewModel.prototype.errorMessage("Error in adding user to group:" + exception.responseText);
                    setTimeout(function () { ViewModel.prototype.errorMessage(""); }, 5000);
                }
            }).done(function (data) { 
            });
            $(event.target).prev().val(''); //Not clearing the text box. 
            $('#acSubInfoUser').val(''); //this is also not clearing 
        };
        /////////////////////////////////////////////////////////////////////////////////
        ////////////CHAIR SAVIND
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectChair = function (event, ui) {
            //Get Gid now
            var gId = 0; 
            ViewModel.prototype.errorMessage = ko.observable();
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
                //alert($("#Prim" + company.replace(/\s/g, '')).length + $("#Prim" + company.replace(/\s/g, '')).val());
                if (txtprim.val().length > 0) {
                    $('#spnMessage').html("You already have a primary representative for " + company + ".  Select another company chair or delete the contact below.");
                    return false;
                }
                else { 
                    $("#Prim" + company.replace(/\s/g, '')).attr("disabled", "disabled");
                }
            }
            if (!(company.length > 0)) {
                alert("No company name found for Chair. So the selection could not be completed");
                return false;
            }
            var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Chair": true, GroupId: ViewModel.prototype.gId() }];
            var dataObject = ko.utils.stringifyJson(temp);
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                    $('#spnMessage').html(ui.item.label + "has been successfully saved as Chair!!"); 
                },
                error: function (exception) {
                    $('#spnMessage').html("Error in saving Group Chair" + exception.responseText);
                }
            }).done(function (data) { 
            }); 
        };
        /////////////////////////////////////////////////////////////////////////////////
        ////////////Vice CHAIR SAVING
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.selectViceChair = function (event, ui) {
            //Get Gid now
            var gId = 0;
            if (isNaN(ViewModel.prototype.gId()))
            { gId = $('#hdnGroupId').val(); }
            else
                gId = ViewModel.prototype.gId();
            ViewModel.prototype.errorMessage = ko.observable();
            //alert(ui.item.label + ui.item.label.indexOf('(') + ui.item.label.substr(ui.item.label.indexOf('(') + 1) + "  " + ui.item.label.slice(ui.item.label.indexOf('(') + 1, -1));
            var company = '';
            if (ui.item.label) {
                company = ui.item.label.trim().slice(ui.item.label.indexOf('(') + 1, -1);
                $('#hdnViceChairComp').val(company);

                var txtprim = $("#Prim" + company.replace(/\s/g, ''));
                //alert($("#Prim" + company.replace(/\s/g, '')).length + $("#Prim" + company.replace(/\s/g, '')).val());
                if (txtprim.val().length > 0) {
                    $('#spnMessage').html("You already have a primary representative for " + company + ".  Select another company vice chair or delete the contact below.");

                    return false;
                }
                else {
                    $("#Prim" + company.replace(/\s/g, '')).attr("disabled", "disabled");
                }

            }
            if (!(company.length > 0))
            {
                alert("No company name found for Vice Chair. So the selection could not be completed");
                retun;
            }
            var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "ViceChair": true, GroupId: ViewModel.prototype.gId() }];
            var dataObject = ko.utils.stringifyJson(temp);
            $.ajax({
                url: '/api/UserGroup',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                    $('#spnMessage').html(ui.item.label + "has been successfully saved as Vice Chair!!");
                    $('#hdnPrimary').val();
                },
                error: function (exception) {
                    //alert('Error in adding user to group:' + exception.responseText);
                    $('#spnMessage').html("Error in saving Group Vice Chair" + exception.responseText);
                }
            }).done(function (data) {
            });
        };
        /////////////////////////////////////////////////////////////////////////////
        /***   Subscribe/Informational - Add all users to Group - not being used ***/
        ////////////////////////////////////////////////////////////////////////////
        ViewModel.prototype.addGroup = function () {
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
                    return false;
                }
            });
        };
        //////////////////////////////////////////////////////////////////////////////
        /***   Subscribe/Informational - Delete User from UserGroup in the DB     ***/
        //////////////////////////////////////////////////////////////////////////////
        ViewModel.prototype.removeUMContacts = function (contact) {
            var dataObject = ko.utils.stringifyJson(contact);
            dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null

            $.ajax({
                url: '/api/RemoveGroupUser',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                    ViewModel.prototype.InfoMessage("User removed from the group successfully!");
                    setTimeout(function () { ViewModel.prototype.InfoMessage(""); }, 3000);
                
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    ViewModel.prototype.errorMessage("Error in removing user" + exception.responseText);
                    return false;
                }
            });
            ViewModel.prototype.selectedValues.remove(contact);
        };
        /////////////////////////////////////////////////////////////////////
        /***   Subscribe/Informational - LOAD Users from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.load = function (groupid, roleid) {
           // ("in tsk load - gid:" + groupid + ", roleid:" + roleid);
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
                        temp['CompanyName'] = '('+result[i].CompanyName+')';
                        temp['UserId'] = result[i].UserId;
                        temp[result[i].Role] = true;
                        temp['GroupId'] = result[i].GroupId; 
                        ViewModel.prototype.selectedValues.push(temp);
                    }
                },
                error: function (exception) { ViewModel.prototype.errorMessage('Exception in Subscribe/Informational user list:' + exception.responseText); }
            });
        }
        /////////////////////////////////////////////////////
        /**Council Committee - Remove Chair /ViceChair**/
        //////////////////////////////////////////////////////
        ViewModel.prototype.removeChairGroupUser = function (item, event) {
            if (isNaN(ViewModel.prototype.gId()))
            { gId = $('#hdnGroupId').val(); }
            else
                gId =ViewModel.prototype.gId();
            var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 3
            //Delete the user from database and clear the field
            ViewModel.prototype.RemoveGroupUser(URL, '' ,'Chair');
            $(event.target).prev().val('');
        }
        ViewModel.prototype.removeViceChairGroupUser = function (item, event) {
            if (isNaN(ViewModel.prototype.gId()))
            { gId = $('#hdnGroupId').val(); }
            else
                gId = ViewModel.prototype.gId();

            var URL = "/api/RemoveGrpUsrbyRole/?GroupId=" + gId + "&RoleId=" + 4;
            //Delete the user from database and clear the field
            ViewModel.prototype.RemoveGroupUser(URL,'','Vice Chair');
            $(event.target).prev().val('');
        }
        ViewModel.prototype.RemoveGroupUser = function (URL) {
            $.ajax({
                url: URL,
                type: 'post',
                contentType: 'application/json',
                success: function (data) {
                    $('#spnMessage').html("User removed successfully!!"); 
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    //ViewModel.prototype.errorMessage("Error:" + xhr.responseText);
                    $('#spnMessage').html("Error removing user!:" + xhr.responseText);
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
        { id: "20", name: "Administrator" },
        { id: "3", name: "Chair" },
        { id: "15", name: "Co-chair" },
        { id: "16", name: "FAA-chair" },
        { id: "17", name: "FAA-Co-chair" },
        { id: "19", name: "Observer" }, 
        { id: "8", name: "Secretary" },
        { id: "6", name: "Spokesperson" }, 
        { id: "12", name: "Subscribe" },
         { id: "18", name: "Team Leader" },  
        { id: "4", name: "Vice Chair" }]);

      
        //20	Administrator
        //3	Chair
        //15	Co-chair
        //16	FAA-Chair
        //17	FAA-Co-Chair
        //19	Observer
        //8	Secretary
        //6	Spokesperson
        //12	Subscribe
        //18	Team Leader
        //4	Vice Chair


        ViewModel.prototype.role = ko.observable();


        ViewModel.prototype.taskRoles = ko.observableArray();
        ViewModel.prototype.gettaskRoles = function () {
            $.ajax({
                url: '/api/GetCommitteRolesbyGType',
                type: "GET",
                datatype: "json",
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    for (key in result) {
                        alert(key + result[key]);
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

        ViewModel.prototype.GetAllUsers = function (request, response) {
            ViewModel.prototype.errorMessage();
            var text = request.term;
            $.ajax({
                url: '/api/GetAllContacts',
                type: 'GET',
                cache: false,
                data: request,
                dataType: 'json',
                success: function (json) {
                    response($.map(json, function (key, value) {
                        return {
                            label: key,
                            text: value,
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
            //Get Gid now
            var gId = 0;
            gId = $('#hdnGroupId').val();
            ViewModel.prototype.errorMessage = ko.observable();
            if (!isNaN(gId) || !isNaN(ViewModel.prototype.gId())) {

                var role = ViewModel.prototype.role();
                var roleName = $("#inpGroupRole option[value='" + role + "']").text().replace(/\s/g, '');//ViewModel.prototype.Roles()[role].name;

                if (typeof role === 'undefined' || ViewModel.prototype.gId() < 1) {
                    ViewModel.prototype.errorMessage("Role or GroupId Missing. The user cannot be added to the group.");
                }
                else {
                    //Set the item to push to DB & knockout array
                    var temp = {};
                    temp['Name'] = ui.item.label;
                    temp['UserId'] = ui.item.text;
                    temp['role'] = roleName;
                    temp[roleName.replace(/\s/g, '')] = true;
                    temp['CompanyName'] ='';
                    temp['GroupId'] = ViewModel.prototype.gId();

                    var dataObject = ko.utils.stringifyJson(temp);
                    dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
                    $.ajax({
                        url: '/api/UserGroup',
                        type: 'post',
                        data: dataObject,
                        contentType: 'application/json',
                        success: function (data) {
                            //alert("Added data successfully");
                            ViewModel.prototype.errorMessage( ui.item.label + "added to the group:");
                            ViewModel.prototype.selectedTskForUser.push(temp);
                        },
                        error: function (exception) {
                            //alert('Error in adding user to group:' + exception.responseText);
                            ViewModel.prototype.errorMessage("Error adding user to group:" + exception.responseText);
                        }
                    }).done(function (data) { });
                }
            }        
    else {
                ViewModel.prototype.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
    }
  };
        /////////////////////////////////////////////////////////////////////
        /***   taskforce- Delete User from UserGroup in the DB     ***/
        ////////////////////////////////////////////////////////////////////
        ViewModel.prototype.removeUMContacts = function (contact) {
            var dataObject = ko.utils.stringifyJson(contact);
            ViewModel.prototype.errorMessage();
            dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
            $.ajax({
                url: '/api/RemoveGroupUser',
                type: 'post',
                data: dataObject,
                contentType: 'application/json',
                success: function (data) {
                },
                error: function (xhr, status, error) {
                    //var err = eval("(" + xhr.responseText + ")");
                    ViewModel.prototype.errorMessage("Error in removing group user." + xhr.responseText);
                    return false;
                }
            });
            ViewModel.prototype.selectedTskForUser.remove(contact);
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
                    //alert("result.length:" + result.length);
                    for (i = 0; i < result.length; i++) {
                        var temp = {};
                        temp['Name'] = result[i].UserName; 
                        temp['CompanyName'] ='(' + result[i].CompanyName + ')';
                        temp['UserId'] = result[i].UserId;
                        temp['role'] = result[i].Role;
                        temp[result[i].Role.replace(/\s/g, '')] = true;
                        temp['GroupId'] = result[i].GroupId;
                        ViewModel.prototype.selectedTskForUser.push(temp);
                    }
                },
                error: function (exception) { ViewModel.prototype.errorMessage('Exception in loading task force user list:' + exception.responseText); }
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
        //alert("In selectPrimary- hdnChairComp:" + $('#hdnChairComp').val() + " &  params.Company: " + params.Company + ($('#hdnChairComp').val().trim() === params.Company.trim()));

        if ($('#hdnChairComp').val().trim() === params.Company.trim())
        {
            alert(params.Company + " representative has been set as Chair. So cannot add Primary contact for them"); 
            return false;
        }
        if ($('#hdnViceChairComp').val().trim() === params.Company.trim()) {
            alert(params.Company + " representative has been set as Vice Chair. So cannot add Primary contact"); 
            return false;
        }

        var compPrimError = (params.Company).replace(/\s+/g, '') + "PrimError";
        /**Remove Error Message*/
        var primErrItem = $('#' + compPrimError);
        if (typeof primErrItem !== "undefined")
            primErrItem.remove();

        //Get GroupId
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
                alert($('#hdnChairComp').val()+ ui.item.label + " has been successfully added as Primary for " + params.Company);
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                // event.target.parentElement = event.target.parentElement.innerHTML + "<br/><span id='" + compPrimError + "' class='error'><i class='fa fa-times-circle'></i>" + err.Message + "</span>";
                var par = $(event.target).parent();
                par.append("<span id='" + compPrimError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + $(event.target).prev().val() + ' ' + err.Message + "</span>");
                $(event.target).prev().val('');
                return false;
            }
        });
        /* In case of cancelling the selection because of invalid user selection*/
        var loc = $(this).val().indexOf('all')
        if (loc > -1 && loc < 1) { $(this).val(''); return false; }
        else
            $(this).val(ui.item.label);
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
                alert("Removed " + Role + " for " + Company);
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")"); 
                $(event.target).parent().append("<span id='" + compPrimError + "' class='error'><br/><i class='fa fa-times-circle'>&nbsp;</i>" + err.Message + "</span>");
                return false;
            }
        })
    }
    self.selectAlternate = function (event, ui) {
        alert($("#hdnViceChairComp").val()  + " & "+ params.Company);
        var compAltError = (params.Company).replace(/\s+/g, '') + "PrimAlt"; 
        /**Remove Error Message*/
        var altErrItem = $('#' + compAltError);
        if (typeof altErrItem !== "undefined")
            altErrItem.remove();

        var temp = [{ "Name": ui.item.label, "UserId": ui.item.text, "Alternate": true, GroupId: gId, CompanyName: params.Company }];
        var dataObject = ko.utils.stringifyJson(temp);
        $.ajax({
            url: '/api/UserGroup',
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                alert( ui.item.label + " has been successfully added as Alternate for " + params.Company);
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                $(event.target).parent().append("<span id='" + compAltError + "' class='error'><br/><i class='fa fa-times-circle'></i>" + $(event.target).prev().val() + ' ' + err.Message + "</span>");
                $(event.target).prev().val('');
                return false;
            }
        })
    }; 
};
/// END OF Council/Committee
/////////////////////////////////////////////////////////////////////////////
//  Council/Committee GGA
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
    this.selectPrimAltUser4Company = function (event, ui) {
        if (!isNaN($('#hdnGroupId').val())) {
            self.gId($('#hdnGroupId').val());
            gId = $('#hdnGroupId').val();
        }
        if ((isNaN(gId) || (isNaN(self.gId())))) {
            self.errorMessage("Invalid GroupID. Your changes will not be saved! Contact the IT Administrator!");
        }
        else {
            //self.errorMessage($('#hdnGroupId').val());
            var role = self.role();
            var roleName = $("#inpGGAGroupRole option[value='" + role + "']").text();
            var comp = self.company();
            var CompanyName = $("#inpGGACompanies option[value='" + comp + "']").text();

            if (typeof role === 'undefined' || typeof comp === 'undefined') {
                self.errorMessage("Role or CompanyName is Missing. The user cannot be added to the group.");
            }
            else {

                //Set the item to push to DB & knockout array
                var temp = {};
                temp['Name'] = ui.item.label;
                temp['UserId'] = ui.item.text;
                temp['Type'] = roleName;
                temp[roleName.replace(/\s/g, '')] = true;
                temp['GroupId'] = self.gId();
                temp['CompanyName'] = CompanyName;
                //First check if the user already exists in the array
                var match = ko.utils.arrayFirst(self.people(),
                    function (item) {
                        return ui.item.text === item.UserId();
                    });
                if (!match) {
                    //Now check if its a Primary and some primary already exists for the company
                    var matchPrim = ko.utils.arrayFirst(self.people(),
                    function (item) {
                        return (CompanyName === item.CompanyName() && roleName === "Primary");
                    });
                    if (matchPrim) {
                        self.removeCommitteePrimAlt(matchPrim);
                    }
                    var dataObject = ko.utils.stringifyJson(temp);
                    dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
                    $.ajax({
                        url: '/api/UserGroupGGA',
                        type: 'post',
                        data: dataObject,
                        contentType: 'application/json',
                        success: function (data) {
                            if (roleName === "Primary") {
                                self.people.push(new CommitteePrimAlt(ui.item.label, ui.item.text, roleName, CompanyName, true, false));
                            } else
                                self.people.push(new CommitteePrimAlt(ui.item.label, ui.item.text, roleName, CompanyName, false, true));
                            self.errorMessage("Successfully added the user!");
                        },
                        error: function (xhr, status, error) {
                            self.errorMessage("Error in adding user. " + xhr.responseText);
                        }
                    });
                }
                else {
                    self.errorMessage("The user " + match.Name() + " already exists under " + match.CompanyName() + " ");
                }
                $("#name-search").val('');
            }
        }
    };
    

    self.removeCommitteePrimAlt = function (CommitteePrimAlt) {
        
        if (isNaN(self.gId())) {
            self.errorMessage("GroupId is missing. The user cannot be removed from the group.");
        }
        else {
            self.people.remove(CommitteePrimAlt);
            var role = 1;
            if (CommitteePrimAlt.Type === "Alternate")
                role = 2;
            var URL = "/api/RemoveGrpUsrbyComp/?CompanyName=" + CommitteePrimAlt.CompanyName + "&GroupId=" + self.gId() + "&RoleId=" + role;
            RemoveGroupUser(URL, CommitteePrimAlt.CompanyName, CommitteePrimAlt.Type);
        }
    };
    function RemoveGroupUser(URL) {
        $.ajax({
            url: URL,
            type: 'post',
            contentType: 'application/json',
            success: function (data) {

            },
            error: function (xhr, status, error) {
                self.errorMessage("Error occured in remove group user!"+ xhr.responseText);
            }
        })
    }
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
                    }
                }));
            }
        });
    };
    /////////////////////////////////////////////////////////////////////
    /***   Council Committe GGA- LOAD Users from UserGroup in the DB     ***/
    ////////////////////////////////////////////////////////////////////
   self.load = function (groupid) { 
       var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid + "&RoleId=1,2,3,4";
       self.errorMessage();
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
                        $('#hdnChairComp').val(result[i].CompanyName );
                        alert( $('#hdnChairComp').val());
                    }
                    else
                    if (result[i].RoleId === 4) {
                        $('#GroupViceChairGGA').val(result[i].UserName + ' (' + result[i].CompanyName + ')');
                        $('#hdnViceChairComp').val(result[i].CompanyName);
                    }                                            
                }
            },
            error: function (exception) {self.errorMessage('Exception in loading task force user list:' + exception.responseText); }
        });
    }
}; 
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
        var query = "/api/GetGroupUserlineItem/?GroupId=" + groupid; 
        $.ajax({
            url: query,
            type: "GET",
            contentType: 'application/json',
            success: function (data) {
                var result = JSON.parse(data);
                for (i = 0; i < result.length; i++) {
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
                     }
                     else
                     if (result[i].RoleId === 4) { 
                         $('#GroupViceChair').val(Name + ' (' + t + ')');
                         $('#hdnViceChairComp').val(result[i].CompanyName); 
                     }
                }
            },
            error: function (exception) { ViewModel.prototype.errorMessage('Exception in Subscribe/Informational user list:' + exception.responseText); }
        });
    }
    ko.validation.init({
        //registerExtenders: true,
        //messagesOnModified: true,
        insertMessages: false
    }); //, true);

    if (parseInt(getGID('gid')) > 0) {
        groupRegisterViewModel = {
            addGroupViewModel: new Group(), addA4AStaffViewModel: A4AStaffViewModel,
            UMSubscribeModel: UMSubscribe, addTaskForceWorkGroup: new TaskForceWorkGroup.ViewModel(),
            addCouncilCommitteeGGA: new councilCommitteeGGAViewModel()
        };

        $.when(SetPrimaryAlternate(parseInt(getGID('gid'), 0))).done(setTimeout(function () { ko.applyBindings(groupRegisterViewModel) }, 1000));
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
