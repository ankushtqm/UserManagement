/// <reference path="../Scripts/knockout-3.1.0.js" />
/// <reference path="../Scripts/jquery-1.10.2.js" />


var homeRegisterViewModel;

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