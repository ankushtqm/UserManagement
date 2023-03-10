/// <reference path="../Scripts/knockout-3.1.0.js" />
/// <reference path="../Scripts/jquery-1.10.2.js" />
var userRegisterViewModel;
/////////////////////////////////////////////////////
// TAB 1 - User Properties Registration view model
/////////////////////////////////////////////////////
function User() {
    var self = this;
    // observable are update elements upon changes, also update on element data changes [two way binding] 
    self.UserName = ko.observable();//.extend({ required: "Please enter a Group Name" });
    self.CompanyId = ko.observable(0);
    self.CompanyName = ko.observable();
    self.FirstName = ko.observable();
    self.LastName = ko.observable(); 
    self.JobTitle = ko.observable();
    self.HomePhone = ko.observable('');
    self.OfficePhone = ko.observable(''); 
    self.OfficePhoneExtension = ko.observable('');
    self.MobilePhone = ko.observable('');
    self.PrimaryFax = ko.observable('');
    self.edit = ko.observable(false);

    //---- added 9/4/2017 - Validation
    var patterns = {
        email: /^([\d\w-\.]+@([\d\w-]+\.)+[\w]{2,4})?$/,
        number: /^[1-9][0-9]*$/
    };
    self.test = ko.observable();
    self.step1Validation = ko.validatedObservable([   
    self.UserName.extend({
        required: { message: ' E-mail is required.' },
        pattern: {
            message: 'Must be a valid email',
            params: patterns.email
        }
    }),
    self.CompanyName.extend({ required: { message: 'Enter a valid CompanyName.' } }),
    self.CompanyId.extend({
        required: { message: 'Cannot find the company with the name. Create a ew Company!' } 
    }),
    self.FirstName.extend({ required: { message: ' FirstName is required.' } }),
    self.LastName.extend({ required: { message: ' LastName is required.' } }),
    self.HomePhone.extend({
        validation: {
            validator: function (val) {
                
                return val === null || val === "" || (typeof val === "undefined") || /^(0|[0-9][0-9]*)$/.test(val);
            },
            message: ' Home Phone Number contains digits only!'
        }
    }),
    self.OfficePhone.extend({
        validation: {
            validator: function (val) {
                return val === null || val === "" || (typeof val === "undefined") || /^(0|[0-9][0-9]*)$/.test(val);
            },
            message: ' Office Phone Number contains digits only!'
        }
    }), 
    self.OfficePhoneExtension.extend({
        validation: {
            validator: function (val) {
                return val === null || val === "" || (typeof val === "undefined") || /^(0|[0-9][0-9]*)$/.test(val);
            },
            message: ' Phone Extension contains digits only!'
        }
    }),
    self.MobilePhone.extend({
        validation: {
            validator: function (val) {
                return val === null || val === "" || (typeof val === "undefined") || /^(0|[0-9][0-9]*)$/.test(val);
            },
            message: ' Mobile Phone Number contains digits only!'
        }
    }),
    self.PrimaryFax.extend({
        validation: {
            validator: function (val) {
                return val === null || val === "" || (typeof val === "undefined") || /^(0|[1-9][0-9]*)$/.test(val);
            },
            message: ' Fax Number contains digits only!'
        }
    }),   
    ]);
    self.removebtn = ko.observable(false);
    self.WebPage = ko.observable();
    self.Twitter = ko.observable();
    self.Facebook = ko.observable();
    self.LinkedIn = ko.observable();
    self.GooglePlus = ko.observable();
    self.Pinterest = ko.observable();
    self.ErrorMessage = ko.observable();
    self.InfoMessage = ko.observable(); 
    self.ErrorMessageGM= ko.observable();
    self.InfoMessageGM = ko.observable();
    self.MessageSM = ko.observable();
    self.InfoMessageMembers = ko.observable();
    self.InfoMessageEmail = ko.observable(); 
    self.ProviderUserKey = ko.observable();
    self.UserId = ko.observable();
    self.PreferredName = ko.observable();
    self.Email = ko.observable().extend({ email: true });
    self.Password = ko.observable();
    self.EmailContent = ko.observable(); 
    self.GroupId = ko.observable(); 
    self.GroupName = ko.observable();
    self.RolePlaceHolder = ko.observable("Select Role");
    self.IsActiveContact = ko.observable(true);
    self.isEconUser = ko.observable(false);
    self.isACHUser = ko.observable(false);
    self.isMemberCompany = ko.observable(false);
    self.showCompanyLabel = ko.observable(false);
    self.rdoFuel = ko.observable("No");
    self.IsActiveFF = ko.computed(function() {     
        if (self.rdoFuel() === "Yes")
            return true;
        else
            return false;
    });
    self.rdoMembers = ko.observable("No");
    self.IsActiveMB = ko.computed(function () {
        if (self.rdoMembers() === "Yes")
            return true;
        else
            return false;
    });
    self.rdoACH = ko.observable("No");
    self.IsActiveACH = ko.computed(function () { 
        if (self.rdoACH() === "Yes")
            return true;
        else
            return false;
    });
     
    /***************************************/
    //Set show errors boolean 
    /***************************************/
    self.isNew = ko.observable(true);  //Shows the isActiveContact and Password fields
    self.ShowErr = ko.observable(false);
    self.rdoMembers.subscribe(function () {
        if(self.IsActiveContact() && (!self.isMemberCompany()))
        { 
            self.securityGroupCount();
            if (self.SecurityGrpCnt > 0 && self.rdoMembers() == "No") 
            {
                alert("Can't remove Member Portal access when user is still part of a council, committee, taskforce, working group, industry group or security group! ");
                self.rdoMembers("Yes")
            }
        }
    });
    self.IsActiveContact.subscribe(function () { 
        if (!self.IsActiveContact()) {
            self.rdoFuel("No");
            self.rdoMembers("No");
            self.rdoACH("No");
        } else {
            self.SetMBEnabled();
        }
    });
    self.UserName.subscribe(function () {
        self.removeErrors();
        self.Email(self.UserName());
        if (isEmail(self.UserName()))
        {
            self.UserExists();
        } 
    }); 
    self.FirstName.subscribe(function () {
        self.removeErrors();
        self.PreferredName(self.FirstName() + ' ' + self.LastName());
    });
    self.LastName.subscribe(function () {
        self.removeErrors();
        self.PreferredName(self.FirstName() + ' ' + self.LastName());
    }); 
    self.removeErrors = function () {
        self.ShowErr(false);
    };  
    self.errors = function () {
        ko.validation.group(this);
    };
    self.ShowErr = ko.observable(false);
    self.SecurityGrpCnt = 0;
    self.UserGroups = ko.observableArray([]);  //Array to save User Groups
    /******************************************************/
    /* Save Company ID and Name when autocomplete selected */
    /******************************************************/
    function isEmail(UserName) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(UserName);
    } 

    self.getEconUser = function isEconUser() {
        $.ajax({
            url: '/api/GetIsCurrentUserITorECON/',
            contentType: 'application/json',
            success: function (data) { 
                self.isEconUser(data);                
            }
        });
    } 
    self.getACHUser = function isACHUser() {
        $.ajax({
            url: '/api/GetIsCurrentUserITorACH/',
            contentType: 'application/json',
            success: function (data) {
                self.isACHUser(data);
            }
        });
    } 
    this.selectedCompany = function (event, ui) { 
        self.CompanyId(ui.item.text);
        self.CompanyName(ui.item.label);
        self.SetMBEnabled();

        //John wants the company to be changable until saved.  By NA 11/2/2018
        //$("#txtCompanyName").hide();
        //$("#hypCreateNewCompany").hide();
        //self.showCompanyLabel(true);
    };
    /*********************************************/
    /** Load user if already exisits
    /*********************************************/
    var UserLoaded = false;
    self.SetMBEnabled = function()
    { 
        if (self.CompanyName().toLowerCase().indexOf("non member") < 1 && self.CompanyName().toLowerCase().indexOf("member") > 0) {
            self.isMemberCompany(true);
            self.rdoMembers("Yes");
        }
        else
            self.isMemberCompany(false);
    }

    self.UserExists = function (userId) {
        $('.overlay').show(); 
            var url = "";
            if (self.UserName() && self.UserName().length > 0 && isNaN(userId)) {
                url = "/api/GetIsUserExisting?email=" + self.UserName();
            }
            else {
                url = "/api/GetIsUserExisting?userId=" + userId; 
            }
            if (!UserLoaded) {
                $.getJSON(url, function (data) {
                    if (data.length > 2) {
                        var d = JSON.parse(data);
                        var uid = d[0]["UserId"]; 
                        $('#hdnUserId').val(uid);  //Moved from below. by NA 9/11/2018 - Save useid in hidden field for later use. Ex:- Seciruity group count.
                        self.isNew(false);
                        self.edit(true);
                        if (self.UserName() && self.UserName().length > 0 && isNaN(userId)) { 
                            $('#hdnUserName').val(self.UserName());
                            self.InfoMessage("This contact is already in the system. We have pre-populated this form with any information we have on this user. Please review carefully before finalizing your changes. ");
                        }
                        else
                            if (!isNaN(userId)) {
                                self.UserName(d[0]["Email"]); //Userid is already saved in the hidden field
                                $('#hdnUserName').val(self.UserName());
                            }
                        self.CompanyName(d[0]["CompanyName"]);
                        self.CompanyId(d[0]["CompanyId"]);
                        $("#txtCompanyName").hide();
                        $("#hypCreateNewCompany").hide();
                        self.showCompanyLabel(true);
                        self.FirstName(d[0]["FirstName"]);
                        self.LastName(d[0]["LastName"]);
                        self.JobTitle(d[0]["JobTitle"]);
                        self.HomePhone(d[0]["HomePhone"]);
                        self.OfficePhone(d[0]["OfficePhone"]);
                        self.OfficePhoneExtension(d[0]["OfficePhoneExtension"]);
                        self.MobilePhone(d[0]["MobilePhone"]);
                        self.PrimaryFax(d[0]["PrimaryFax"]);
                        self.WebPage(d[0]["WebPage"]);
                        self.Twitter(d[0]["Twitter"]);
                        self.Facebook(d[0]["Facebook"]);
                        self.LinkedIn(d[0]["LinkedIn"]);
                        self.GooglePlus(d[0]["GooglePlus"]);
                        self.Pinterest(d[0]["Pinterest"]);
                        self.IsActiveContact(d[0]["IsActiveContact"]);
                        if (d[0]["IsActiveFF"])
                            self.rdoFuel("Yes");
                        else
                            self.rdoFuel("No");
                        if (d[0]["IsActiveMB"])
                            self.rdoMembers("Yes");
                        else
                            self.rdoMembers("No");

                        if (d[0]["IsActiveACH"])
                            self.rdoACH("Yes");
                        else
                            self.rdoACH("No");
                        self.Password(d[0]["Password"]);
                        self.FirstName(d[0]["FirstName"]);
                        self.LastName(d[0]["LastName"]);
                        userRegisterViewModel.addUserAddViewModel.Load();
                        self.loadUserGroups();
                        self.SetMBEnabled();
                        self.getEmailContent(); 
                    }
                    else {
                        if (!(self.FirstName() === undefined) && self.FirstName().length > 1) {
                            $("#txtCompanyName").hide();
                            $("#hypCreateNewCompany").hide();
                            self.showCompanyLabel(true);
                            self.FirstName('');
                            self.LastName('');
                            self.JobTitle('');
                            self.HomePhone('');
                            self.OfficePhone('');
                            self.OfficePhoneExtension('');
                            self.MobilePhone('');
                            self.PrimaryFax('');
                            self.WebPage('');
                            self.Twitter('');
                            self.Facebook('');
                            self.LinkedIn('');
                            self.GooglePlus('');
                            self.Pinterest('');
                            self.rdoFuel("No");
                            self.rdoMembers("No");
                            self.rdoACH("No");
                            self.Password('');
                            self.FirstName('');
                            self.LastName('');
                            self.InfoMessage("");
                        }
                    }
                });
                if (!isNaN(userId))
                    UserLoaded = true; 
            }
        $(".overlay").hide();
    }; 
    /********************************************/
    /*  Get Company ajax call */
    /********************************************/
    this.getCompany = function (request, response) { 
        $.ajax({
            url: '/api/GetAllCompanies/',
            type: 'GET',
            cache: false,
            data: request,
            dataType: 'json',
            success: function (json) { 
                if (ko.utils.stringifyJson(json).length < 3) 
                    $(".CompanyValid").css('display','inline-block'); 
                else 
                    $(".CompanyValid").css('display', 'none'); 
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
    self.InActivateUser = function () {
        self.IsActiveContact(false);
        self.resetMessages();
        self.SaveUsertoDB(1);
        self.InfoMessage("This user has been removed from database. He won't show up in the Global Address Book or the User search on the home page.");
    } 
    self.SaveUser = function () {
        self.resetMessages();
        //diabling the email textbox 
        $("#inpEmail").prop( "disabled", true ); 
        var result = ko.validation.group(userRegisterViewModel.addUserViewModel, { deep: true });
       
        if (!self.step1Validation.isValid()) { 
            self.test('invalid');
            self.step1Validation.errors.showAllMessages()
            return false;
        }
        else
        { 
            if (!isNaN(self.CompanyId())) {
                self.ShowErr(false); // Hide errors 
                if (self.UserGroups().length > 0 && !self.IsActiveContact()) { 
                    alert("User still has Groups assigned and IsActive Contact set to FALSE. Remove all groups to InActivate user!");
                    return;
                }
                else {
                    self.securityGroupCount($('#hdnUserId').val());
                    if (self.UserGroups().length > 0 && self.SecurityGrpCnt === 0 && !(self.CompanyName().toLowerCase().indexOf("non member") > 0)) {
                        // self.rdoMembers("No"); 
                      // CHANGE ASKED BY JD - 7/1/2018  alert("We want to be sure you knew that this contact is not a member of a group that will provide Member Portal access.  If you are ok with that, please proceed. Otherwise, go back and subscriber this contact to a group that does provide access.");
                    }
                }
               //**********************************************************//
               // Let user know that no groups are associated with the user 
               //**********************************************************//
               if (!isNaN($('#hdnUserId').val()) && !(self.UserGroups().length > 0)) {
                    //If it is a new group, then don't give an option to remove user
                    if (self.isNew()) 
                        self.SaveUsertoDB(0); 
                    else
                    {
                        if (self.IsActiveContact()) {
                            if (confirm("This users is not part of any groups now. If you select 'OK', it will be saved as inactive. If you select 'Cancel' the contact will continue to stay active.") === true) {
                                self.InActivateUser();
                            } else {                                
                                self.SaveUsertoDB(0);
                            }
                        }
                        else  
                            self.SaveUsertoDB(0); 
                    }
                }
                else  
                    self.SaveUsertoDB(0);  
            } 
            self.InfoMessage('');
         
        } 
    };
    self.SaveUsertoDB = function(type)
    {
        $('.overlay').show();
        var dataObject = ko.toJSON(this);
        self.edit(true);  
        $("#inpEmail").prop("disabled", true);
        $.ajax({
            url: '/api/User?CompanyId=' + self.CompanyId() + '&password=' + self.Password(),
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                if (type === 0)
                    self.InfoMessage("Contact details saved successfully!!"); //Now saving user group membership!");
                else 
                    self.InfoMessage("Contact inactivated successfully!!");

                //Added to hide txtcompany after saving the user was successful 
                $("#txtCompanyName").hide();
                $("#hypCreateNewCompany").hide();
                self.showCompanyLabel(true);

            },
            error: function (exception) {
                $(".overlay").hide();
                self.InfoMessage('');
                self.ErrorMessage("Contact not saved! An error occured -" + exception.responseText);
            }
        }).done(function (data) {
            self.ErrorMessage('');
            //Set the usernames and userid to the hidden fields to carry forward for others
            $('#hdnUserId').val(data.UserId);
            $('#hdnUserName').val(self.UserName());
            self.isNew(false); 
            self.Password(data.Password);
            $("#Headtitle").html("New User Wizard");
            if (!($(".UserTitle").html().indexOf("User:") > 0)) {
                $(".UserTitle").append("<br/> <span class='niceLabel-black margin-topbot10'>User: " + $('#hdnUserName').val() + "</span");
            }
            //Update User Groups
            if (type === 0) { //save groups only if it a user save or update, if it is user remove, which happens only after removing all groups. No need to save the user groups
                 self.getEmailContent();
            }
            $(".overlay").hide();
        });
    } 
    self.load = function (user) {   
        var ATAUser = JSON.parse(user.responseText); 
        self.isNew(false); //Shows the isActiveContact and Password fields  
        $("#inpEmail").prop("disabled", true); 
    };
    self.loadUserGroups = function () {  //delete if everything works 
        if (!isNaN($('#hdnUserId').val())) {
            self.UserGroups.removeAll();
            
            var query = "/api/GetGroupUserlineItem/?UserId=" + $('#hdnUserId').val();
            $.ajax({
                url: query,
                type: "GET",
                datatype: "json",
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var result = JSON.parse(data); 
                    for (i = 0; i < result.length; i++) {
                        var temp = {};
                        temp['GroupName'] = result[i].GroupName;
                        temp['GroupType'] = result[i].GroupType; //"("+ result[i].GroupType + ")";
                        temp['GroupId'] = result[i].GroupId;
                        temp['Role'] = result[i].Role;
                        temp[result[i].Role.replace(/\s/g,'').trim()] = true;
                        temp['CompanyName'] = result[i].CompanyName;
                        temp['UserId'] = $('#hdnUserId').val();
                        if (result[i].IsCurrentUserAdmin > 0) {
                            temp['IsCurrentUserAdmin'] = true;
                        } else {
                            temp['IsCurrentUserAdmin'] = false;
                        }
                        self.UserGroups.push(temp);
                    }
                },
                error: function (exception) { self.ErrorMessage(' ' + exception.responseText); }
            });
        }
        else
            self.ErrorMessage('Could not find the UserId '); 
    };
    /********************************************/
    /* GroupMemberShip Code
       Get roles by GroupID */
    /********************************************/
    self.role = ko.observable();
    self.Roles = ko.observableArray();
    self.Roles.push({
        name: "",// Push the key on the array
        value: "Select a Role" // Push the value of the key on the array
    });
    self.getRoles = function () {
        $.ajax({
            url: '/api/GetRolesbyGroupID/?GroupId=' + self.GroupId(),
            type: "GET",
            datatype: "json",
            processData: false,
            contentType: "application/json; charset=utf-8", 
            success: function (result) {
                self.Roles.removeAll();

                for (key in result) { 
                    var item = {
                        name: key,// Push the key on the array
                        value: result[key] // Push the value of the key on the array
                    };
                    self.Roles.push(item);
                }
            },  
            error: function (exception) {
                self.ErrorMessageGM("Error loading roles for group:" + exception.responseText); 
                self.InfoMessageGM('');
            }
        });
    };
    self.clearUserGroups = function () {
        $("#usersGrps").val("");
    };
    /*********************************/
    /*****Get Groups */
    /*********************************/
    self.GetAllGroups = function (request, response) { 
        var text = request.term; 
        $.ajax({
            url: '/api/GetAllGroups/?CompanyId=' + self.CompanyId() + '&isActiveMB=' + self.IsActiveMB(),
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
              },  
             error: function (exception) { 
                 self.InfoMessageGM('');
            }            
        });
    };
    self.UpdateUserIdtoUserGroups = function () { 
        for (i = 0; i < self.UserGroups().length; i++) {
            self.UserGroups()[i]['UserId'] = $('#hdnUserId').val(); //self.UserGroups()[0]['UserId']
        }
    };
    /********************************************************
    // Adding group to UserGroup array for it to be saved later
    **********************************************************/
    self.AddGroup = function () {
        if (self.IsActiveContact()) {
            self.resetMessages();
            var roleName = $("#inpGroupRole option[value='" + self.role() + "']").text().replace(/\s/g, '');
            var Uid = $('#hdnUserId').val();
            var match = ko.utils.arrayFirst(self.UserGroups(),
                       function (item) {
                           if (self.GroupName().indexOf(item.GroupName) > 0) {
                               return roleName == item.Role
                           };
                       });
            if (!match) {
                if (roleName.length > 1 && !isNaN(self.GroupId())) {
                    var temp = {}; 
                    var startindex = self.GroupName().indexOf('(');
                    var gtype = self.GroupName().substring(startindex + 1).replace(')', ''); //Get the GroupType out of the Group Name.
                    var grpname = self.GroupName().replace(self.GroupName().substring(startindex), '');

                    temp['GroupName'] = grpname;
                    temp['GroupType'] = gtype;
                    temp['GroupId'] = self.GroupId();
                    temp['Role'] = roleName;
                    temp[roleName.replace(/\s/g, '').trim()] = true; //Replace space from roleName so it can work with the class properties when posted back.
                    temp['CompanyName'] = '';
                    temp['UserId'] = $('#hdnUserId').val();
                    temp['IsCurrentUserAdmin'] = true;
                    
                    var dataObject = ko.utils.stringifyJson(temp); 
                    dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
                    $.ajax({
                        url: '/api/UserGroup',
                        type: 'post',
                        data: dataObject,
                        contentType: 'application/json',
                        success: function (data) {
                            self.UserGroups.push(temp);
                            self.InfoMessageGM(self.GroupName() + roleName + " - Contact added to the group successfully!");
                            setTimeout(function () { self.InfoMessage(""); }, 4000); 
                        },
                        error: function (exception) {
                            self.ErrorMessageGM("Error in adding contact to group:" + exception.responseText);
                            setTimeout(function () { self.ErrorMessageGM(""); }, 6000);
                        }
                    });  
                    if (!self.IsActiveContact())
                        self.IsActiveContact(true);//If user is inactive, make him active because now he had groups added
                }
                else {
                    self.ErrorMessageGM("GroupId or RoleName not captured correctly!");
                    setTimeout(function () { self.ErrorMessageGM(""); }, 4000);
                }
            }
            else {
                self.ErrorMessageGM("Group <span class='text-info'>" + self.GroupName() + "</span> along with the role <span class='text-info'>" + roleName + "</span>  has already been added to the UserGroups. Choose a different group or role!");
                setTimeout(function () { self.ErrorMessage(""); }, 4000);
            }
        }
        else
        {
            self.ErrorMessageGM("Contact is inactive. Activate contact by checking the box in group details screen and then hitting the 'Save Details' button so you can add user to the groups!");
            setTimeout(function () { self.ErrorMessage(""); }, 4000);
        } 
    };
    self.SaveGroup = function () {
        $('.overlay').show();
        var dataObject = ko.utils.stringifyJson(self.UserGroups()); 
        $.ajax({
            url: '/api/UserGroup/',
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                self.InfoMessageGM("Contact's Group Membership has been saved successfully");
                setTimeout(function () { self.InfoMessageGM(""); }, 4000);
                self.InfoMessage("All Contact Details, Membership and User Groups have been saved successfully!!");
                setTimeout(function () { self.InfoMessage(""); }, 4000);
            },
            error: function (xhr, status, error) {
                if (xhr.responseText.indexOf("Contact is already a member of this Group. Remove group and save again.") === 0) {
                    self.ErrorMessageGM( xhr.responseText);
                    setTimeout(function () { self.ErrorMessageGM(""); }, 4000);
                }

                return false;
            }
        }).done(function (data) {
            $(".overlay").hide(); 
        });
    };
    self.SelectGroupforUser = function (event, ui) {
        self.GroupId(ui.item.text); 
        self.GroupName(ui.item.label);  //ui.item.label 
        self.getRoles();
    };
    /**************************************************************
    // Get Security Group Count
    ***************************************************************/
    self.securityGroupCount = function (uid) {
        if (uid === undefined || uid == null  || !(uid > 0))
        {
            uid = $('#hdnUserId').val();
        }
        var group = $.ajax({
            url: '/api/isSecurityGroupCount/?uid=' + uid,
            async: false,
            success: function (data) { 
                self.SecurityGrpCnt = data; 
            },
            error: function (xhr, status, error) {
                return error;
            }          
            });
    };
    self.removeUserGroup = function (u) { 
        //Removing the Count for the Security groups 
        var dataObject = ko.utils.stringifyJson(u); 
        dataObject = "[" + dataObject + "]"; //Otherwise the dataobject at the API is null
        $.ajax({
            url: '/api/RemoveGroupUser',
            type: 'post',
            data: dataObject,
            contentType: 'application/json',
            success: function (data) {
                self.InfoMessageGM("Successfully removed the user group!");
                setTimeout(function () { self.InfoMessageGM(""); }, 4000);
            },
            error: function (xhr, status, error) { 
                self.ErrorMessageGM("Error in removing group user." + xhr.responseText);
                setTimeout(function () { self.ErrorMessageGM(""); }, 4000);
                return false;
            }   });
        self.UserGroups.remove(u); 
        if (self.UserGroups().length < 1)
        {
            if (confirm("This users is not part of any groups now. If you select 'OK', it will be saved as inactive. If you select 'Cancel' the contact will continue to stay active.") === true) {
                self.InActivateUser();
            } else {
                self.SaveUsertoDB(0);
            }
        }
    };
     self.resetMessages = function ()
    {
        self.InfoMessage('');
        self.InfoMessageGM('');
        self.ErrorMessage('');
        self.ErrorMessageGM('');
    }; 
    ///***********************************************************/
    /* EMAIL NOTIFICATION SECTION ***/
    ///***********************************************************/

     self.setEmailType = function () {
         self.emailType = 0;
        if (self.IsActiveContact() && self.IsActiveMB() && !self.IsActiveFF() && !self.IsActiveACH()) {
            self.emailType = 1;
        }
        if (self.IsActiveContact() && !self.IsActiveMB() && self.IsActiveFF() && !self.IsActiveACH()) {
            self.emailType = 2;
        }
        if (self.IsActiveContact() && !self.IsActiveMB() && !self.IsActiveFF() && self.IsActiveACH()) {
            self.emailType = 4;
        }
        if (self.IsActiveContact() && self.IsActiveMB() && self.IsActiveFF() && !self.IsActiveACH()) {
            self.emailType = 3;
        }
        if (self.IsActiveContact() && self.IsActiveMB() && !self.IsActiveFF() && self.IsActiveACH()) {
            self.emailType = 5;
        }
        if (self.IsActiveContact() && !self.IsActiveMB() && self.IsActiveFF() && self.IsActiveACH()) {
            self.emailType = 6;
        }
        if (self.IsActiveContact() &&  self.IsActiveMB() && self.IsActiveFF() && self.IsActiveACH()) {
            self.emailType = 7;
        }
    };
    self.getEmailContent = function () {
        self.setEmailType(); 
        switch (self.emailType) {
            case 1: self.EmailContent(" <p>Welcome to the A4A Member Portal. Below you will find your username and temporary password for access to the site. " +
                                      " Please click the \"Login\" link below. </p>" +
                                      "  <br/>" +
                                      "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                      "  <br/>" +
                                      "  <p>   Username: " + self.Email() + " </p>" +
                                      "  <p>   Password: ********* </p>" + 
                                      " <a href=\"http://memberportal.airlines.org/\" title='Member Portal Login'>Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
            case 2: self.EmailContent(" <p>Welcome to the A4A Fuel Portal. Below you will find your username and temporary password for access to the site. " +
                                      " Please click the\"Login\" link below. </p>" +
                                      "  <br/> " +
                                      "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                      "  <br/> " +
                                      "  <p>   Username: " + self.Email() + " </p>" +
                                      "  <p>   Password: ********* </p>" +
                                      //"  <p>   Password: " + self.Password() + "</p>" +
                                      " <a href=\"http://fuelportal.airlines.org/\" title='Fuel Portal Login'>Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
            case 3: self.EmailContent(" <p>Welcome to the A4A Member Portal and Fuel Portal. Below you will find your username and temporary password for access to the site. Please click the “Member Portal Login” or “Fuel Portal Login” link below.</p>" +
                                     "  <br/> " +
                                     "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                     "  <br/> " +
                                     "  <p>   Username: " + self.Email() + " </p>" +
                                     //"  <p>   Password: " + self.Password() + "</p>" +
                                     "  <p>   Password: ********* </p>" +
                                     " <a href=\"http://fuelportal.airlines.org/\" title='Fuel Portal Login'>Fuel Portal Login</a> |" +
                                     " <a href=\"http://memberportal.airlines.org/\" title='Member Portal Login'>Member Portal Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
            case 4: self.EmailContent(" <p>Welcome to the ACH Portal. Below you will find your username and temporary password for access to the site. " +
                                     " Please click the \"Login\" link below. </p>" +
                                     "  <br/> " +
                                     "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                     "  <br/> " +
                                     "  <p>   Username: " + self.Email() + " </p>" +
                                     "  <p>   Password: ********* </p>" +
                                     //"  <p>   Password: " + self.Password() + "</p>" +
                                     " <a href=\"http://memberportal.airlines.org/ach/\" title='ACH Portal Login'>Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
            case 5: self.EmailContent(" <p>Welcome to the A4A Member Portal and ACH Portal. Below you will find your username and temporary password for access to the site. " +
                                     " Please click the \"Login\" link below. </p>" +
                                     "  <br/>" +
                                     "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                     "  <br/>" +
                                     "  <p>   Username: " + self.Email() + " </p>" +
                                     "  <p>   Password: ********* </p>" +
                                     " <a href=\"http://memberportal.airlines.org/\" title='Member Portal Login'> Member Portal Login</a> |" +
                                     " <a href=\"http://memberportal.airlines.org/ach/\" title='ACH Portal Login'> ACH Portal Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
              case 6: self.EmailContent(" <p>Welcome to the A4A Fuel Portal and ACH Portal. Below you will find your username and temporary password for access to the site. " +
                                     " Please click the \"Login\" link below. </p>" +
                                     "  <br/>" +
                                     "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                     "  <br/>" +
                                     "  <p>   Username: " + self.Email() + " </p>" +
                                     "  <p>   Password: ********* </p>" +
                                     " <a href=\"http://fuelportal.airlines.org/\" title='Fuel Portal Login'> Fuel Portal Login</a> |" +
                                     " <a href=\"http://memberportal.airlines.org/ach/\" title='ACH Portal Login'> ACH Portal Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
            case 7: self.EmailContent(" <p>Welcome to the A4A Member Portal, A4A Fuel Portal, and ACH Portal. Below you will find your username and temporary password for access to the site. " +
                                " Please click the \"Login\" link below. </p>" +
                                "  <br/>" +
                                "  <p> Once you log in with your temporary password, you will be prompted to change it. </p>" +
                                "  <br/>" +
                                "  <p>   Username: " + self.Email() + " </p>" +
                                "  <p>   Password: ********* </p>" +
                                " <a href=\"http://memberportal.airlines.org/\" title='Member Portal Login'> Member Portal Login</a> |" +
                                " <a href=\"http://fuelportal.airlines.org/\" title='Fuel Portal Login'> Fuel Portal Login</a> |" +
                                " <a href=\"http://memberportal.airlines.org/ach/\" title='ACH Portal Login'> ACH Portal Login</a>");
                $('#z-tab3').show();
                self.MessageSM("");
                break;
            default: self.EmailContent("");
                     self.MessageSM("User is not active member of any sites. So an email wont be sent out.");
                     $('#z-tab3').hide();
                     break; 
        }
    }; 
    self.SendEmail = function () {
        self.InfoMessageEmail("Sending E-Mail to the user....");
        var username = $('#hdnUserName').val().trim();

        if ($('#hdnUserName').val().trim().length > 0) {
            var temp = {};
            body = $("#divEmailContent").html().replace("*********", self.Password()); //EmailContent Knockout changed value is not reflecting. so had to pull it with hardcore jquery. //TODO: Clean it

            to = username; //"ITdev@airlines.org"; //ITDev
            from = "userManagement@airlines.org";
            subject = "Your A4A User Management Account";
            bodyIsHtml = true; 
            var url = "/api/SendEmail/?body=" + body + "&to=" + to + "&from=" + from + "&subject=" + subject + "&bodyIsHtml=" + bodyIsHtml;
          
            $.ajax({
                url: url,
                type: 'post',
                contentType: 'application/json',
                success: function (data) {
                    self.MessageSM('');
                    self.InfoMessageEmail("Confirmation Email has been sent successfully!!");
                },
                error: function (xhr, status, error) {
                    self.MessageSM("Error in sending confirmation email." + xhr.responseText);
                    return false;
                }
            });
        }
        else {
            self.InfoMessageEmail("");
            self.MessageSM("Error in sending confirmation email. Couldnot find the email address");

        }
    };
    ///***********************************************************/
    /* END of EMAIL NOTIFICATION SECTION ***/
    ///***********************************************************/ 
}
 setChosen = function () { 
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
/*****************************************************************/
/********** Address Class **/
/*****************************************************************/
 var Address = function (AddressId, AddressType, Address1, Address2, City, State, ZipCode, Province, Country) {
     this.AddressId = ko.observable(AddressId);
     this.AddressType = ko.observable(AddressType);
     this.Address1 = ko.observable(Address1);
     this.Address2 = ko.observable(Address2);
     this.City = ko.observable(City);
     this.State = ko.observable(State);
     this.ZipCode = ko.observable(ZipCode);
     this.Province = ko.observable(Province);
     this.Country = ko.observable(Country); 
 }

/**********************************************************************/
 var userAddress = function () {
     var self = this;
     self.uId = ko.observable($('#hdnUserId').val());
     var uId = $('#hdnUserId').val();
     self.ShowErr = ko.observable(false);
     self.Addresses = ko.observableArray();
     self.ErrorMessage = ko.observable();
     self.InfoMessage = ko.observable();
     self.AddressType = ko.observable(); 
     self.Address1 = ko.observable('');
     self.Address2 = ko.observable('');
     self.City = ko.observable('');
     self.State = ko.observable('');
     self.ZipCode = ko.observable(''); 

     self.Province = ko.observable('');
     self.Country = ko.observable('');

 self.step2Validation = ko.validatedObservable([
 self.Address1.extend({ required: { message: ' Address1 is required.' } }), 
 
 ]);
 self.addUserAddress = function () {
         var result = ko.validation.group(userRegisterViewModel.addUserAddViewModel, { deep: true }); 
         $(".overlay").show(); 

         if (!self.step2Validation.isValid()) {  
             self.step2Validation.errors.showAllMessages(); 
             $(".overlay").hide();
             return false;
         }
         else
         {
             if ($('#hdnUserName').val().trim().length > 0) {
                 var dataObject = ko.toJSON(this);
                 $.ajax({
                     url: '/api/SaveAddress/?addressType=' + 1 + '&email=' + $('#hdnUserName').val(),
                     type: 'post',
                     data: dataObject,
                     contentType: 'application/json',
                     success: function (data) {
                         $(".overlay").hide(); 
                         self.InfoMessage("User Address saved successfully!");
                         setTimeout(function () { self.InfoMessage(""); }, 4000);
                         self.Addresses.push(new Address(0, 1, self.Address1(), self.Address2(), self.City(), self.State(), self.ZipCode(), self.Province(), self.Country()));

                         self.Address1("");
                         self.Address2("");
                         self.City(""); 
                         self.State(""); 
                         self.ZipCode(""); 
                         self.Province("");
                         self.Country(""); 
                         self.step2Validation.errors.showAllMessages(false);
                     },
                     error: function (exception) {
                         $(".overlay").hide(); 
                         self.ErrorMessage("Error occured while saving User Address!" + exception.responseText);
                         setTimeout(function () { self.InfoMessage(""); }, 4000);

                     }
                 }).done();
             }
         }
         $(".overlay").hide();
     };
     self.Load = function () {
         var query = "/api/getuseraddress/?UserId=" + $('#hdnUserId').val(); 
         $.ajax({
             url: query,
             type: "GET",
             datatype: "json",
             processData: false,
             contentType: "application/json; charset=utf-8",
             success: function (result) {
                 var add = JSON.parse(result); 
                 for (i = 0; i < add.length; i++) {
                   self.Addresses.push(new Address(add[i].AddressId,
                          add[i].AddressTypeId,//264,
                        add[i].Address1,
                        add[i].Address2,
                       add[i].City, add[i].State, add[i].ZipCode, add[i].Province, add[i].Country
                     ));
                 }
             },
             error: function (exception) { self.ErrorMessage('Exception in loading addresses:' + exception.responseText); }
         });
     }; 
     self.removeAddress = function (add) { 
         $.ajax({
             url: '/api/RemoveAddress?AddressId=' + add.AddressId() + '&UserId=' + $('#hdnUserId').val(),
             type: 'post',
             data: add,
             contentType: 'application/json',
             success: function (data) {
                 self.InfoError("Address deleted successfully!");
             },
             error: function (xhr, status, error) {
                 self.ErrorMessage(xhr.responseText);
                 return false;
             }
         });
         self.Addresses.remove(add);
     };
 }
/////////////////////////////////////////////////////////////////////
/*******TAB  ko_autocomplete  ********/
/////////////////////////////////////////////////////////////////////  
ko.bindingHandlers.ko_autocomplete = {
    init: function (element, params) {
        $(element).autocomplete(params());
    },
    update: function (element, params) {
        $(element).autocomplete("option", "source", params().source);
    }
};
/**************************************************************/
/*** Distinct function
/**************************************************************/
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
/*******Global KO binding Object********/
/////////////////////////////////////////////////////////////////////
 
$(document).ready(function () {
    //Clear session 
   sessionStorage.clear();
   $('.editable').each(function () {
        this.contentEditable = true;
   });

   function getUID(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'), sParameterName, i;
        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
   }

   ko.validation.init({
       insertMessages: false
   });

   var patterns = {
       email: /^([\d\w-\.]+@([\d\w-]+\.)+[\w]{2,4})?$/
   }; 
   if (parseInt(getUID('uid')) > 0) {
        userRegisterViewModel = {
            addUserViewModel: new User(), addUserAddViewModel: new userAddress()
        };
        ko.applyBindings(userRegisterViewModel); 
        $.when(userRegisterViewModel.addUserViewModel.getCompany(), userRegisterViewModel.addUserViewModel.getEconUser(), userRegisterViewModel.addUserViewModel.getACHUser()).done();
   }
    else {
        userRegisterViewModel = {
            addUserViewModel: new User(), addUserAddViewModel: new userAddress()
        };
        ko.applyBindings(userRegisterViewModel); 
        $.when(userRegisterViewModel.addUserViewModel.getCompany(), userRegisterViewModel.addUserViewModel.getEconUser(), userRegisterViewModel.addUserViewModel.getACHUser()).done();
    }
});
