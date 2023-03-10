/*
 * @license
 *
 * Multiselect v2.3.1
 * http://crlcu.github.io/multiselect/
 *
 * Copyright (c) 2016 Adrian Crisan
 * Licensed under the MIT license (https://github.com/crlcu/multiselect/blob/master/LICENSE)
 */
if("undefined"==typeof jQuery)throw Error("multiselect requires jQuery");(function($){"use strict";var version=$.fn.jquery.split(" ")[0].split(".");if(2>version[0]&&7>version[1])throw Error("multiselect requires jQuery version 1.7 or higher")})(jQuery),function(factory){"function"==typeof define&&define.amd?define(["jquery"],factory):factory(jQuery)}(function($){"use strict";var Multiselect=function($){function Multiselect($select,settings){var id=$select.prop("id");this.$left=$select,this.$right=$(settings.right).length?$(settings.right):$("#"+id+"_to"),this.actions={$leftAll:$(settings.leftAll).length?$(settings.leftAll):$("#"+id+"_leftAll"),$rightAll:$(settings.rightAll).length?$(settings.rightAll):$("#"+id+"_rightAll"),$leftSelected:$(settings.leftSelected).length?$(settings.leftSelected):$("#"+id+"_leftSelected"),$rightSelected:$(settings.rightSelected).length?$(settings.rightSelected):$("#"+id+"_rightSelected"),$undo:$(settings.undo).length?$(settings.undo):$("#"+id+"_undo"),$redo:$(settings.redo).length?$(settings.redo):$("#"+id+"_redo"),$moveUp:$(settings.moveUp).length?$(settings.moveUp):$("#"+id+"_move_up"),$moveDown:$(settings.moveDown).length?$(settings.moveDown):$("#"+id+"_move_down")},delete settings.leftAll,delete settings.leftSelected,delete settings.right,delete settings.rightAll,delete settings.rightSelected,delete settings.undo,delete settings.redo,delete settings.moveUp,delete settings.moveDown,this.options={keepRenderingSort:settings.keepRenderingSort,submitAllLeft:void 0!==settings.submitAllLeft?settings.submitAllLeft:!0,submitAllRight:void 0!==settings.submitAllRight?settings.submitAllRight:!0,search:settings.search,ignoreDisabled:void 0!==settings.ignoreDisabled?settings.ignoreDisabled:!1},delete settings.keepRenderingSort,settings.submitAllLeft,settings.submitAllRight,settings.search,settings.ignoreDisabled,this.callbacks=settings,this.init()}return Multiselect.prototype={init:function(){var self=this;self.undoStack=[],self.redoStack=[],(self.$left.find("optgroup").length||self.$right.find("optgroup").length)&&(self.callbacks.sort=!1),self.options.keepRenderingSort&&(self.skipInitSort=!0,self.callbacks.sort!==!1&&(self.callbacks.sort=function(a,b){return $(a).data("position")>$(b).data("position")?1:-1}),self.$left.find("option").each(function(index,option){$(option).data("position",index)}),self.$right.find("option").each(function(index,option){$(option).data("position",index)})),"function"==typeof self.callbacks.startUp&&self.callbacks.startUp(self.$left,self.$right),self.skipInitSort||"function"!=typeof self.callbacks.sort||(self.$left.mSort(self.callbacks.sort),self.$right.each(function(i,select){$(select).mSort(self.callbacks.sort)})),self.options.search&&self.options.search.left&&(self.options.search.$left=$(self.options.search.left),self.$left.before(self.options.search.$left)),self.options.search&&self.options.search.right&&(self.options.search.$right=$(self.options.search.right),self.$right.before($(self.options.search.$right))),self.events()},events:function(){var self=this;self.options.search&&self.options.search.$left&&self.options.search.$left.on("keyup",function(){this.value?(self.$left.find('option:search("'+this.value+'")').mShow(),self.$left.find('option:not(:search("'+this.value+'"))').mHide(),self.$left.find("option.hidden").parent("optgroup").not($(":visible").parent()).mHide(),self.$left.find("option:not(.hidden)").parent("optgroup").mShow()):self.$left.find("option, optgroup").mShow()}),self.options.search&&self.options.search.$right&&self.options.search.$right.on("keyup",function(){this.value?(self.$right.find('option:search("'+this.value+'")').mShow(),self.$right.find('option:not(:search("'+this.value+'"))').mHide(),self.$right.find("option.hidden").parent("optgroup").not($(":visible").parent()).mHide(),self.$right.find("option:not(.hidden)").parent("optgroup").mShow()):self.$right.find("option, optgroup").mShow()}),self.$right.closest("form").on("submit",function(){self.$left.find("option").prop("selected",self.options.submitAllLeft),self.$right.find("option").prop("selected",self.options.submitAllRight)}),self.$left.on("dblclick","option",function(e){e.preventDefault();var $options=self.$left.find("option:selected");$options.length&&self.moveToRight($options,e)}),self.$left.on("keypress",function(e){if(13===e.keyCode){e.preventDefault();var $options=self.$left.find("option:selected");$options.length&&self.moveToRight($options,e)}}),self.$right.on("dblclick","option",function(e){e.preventDefault();var $options=self.$right.find("option:selected");$options.length&&self.moveToLeft($options,e)}),self.$right.on("keydown",function(e){if(8===e.keyCode||46===e.keyCode){e.preventDefault();var $options=self.$right.find("option:selected");$options.length&&self.moveToLeft($options,e)}}),(navigator.userAgent.match(/MSIE/i)||navigator.userAgent.indexOf("Trident/")>0||navigator.userAgent.indexOf("Edge/")>0)&&(self.$left.dblclick(function(){self.actions.$rightSelected.trigger("click")}),self.$right.dblclick(function(){self.actions.$leftSelected.trigger("click")})),self.actions.$rightSelected.on("click",function(e){e.preventDefault();var $options=self.$left.find("option:selected");$options.length&&self.moveToRight($options,e),$(this).blur()}),self.actions.$leftSelected.on("click",function(e){e.preventDefault();var $options=self.$right.find("option:selected");$options.length&&self.moveToLeft($options,e),$(this).blur()}),self.actions.$rightAll.on("click",function(e){e.preventDefault();var $options=self.$left.children(":not(span):not(.hidden)");$options.length&&self.moveToRight($options,e),$(this).blur()}),self.actions.$leftAll.on("click",function(e){e.preventDefault();var $options=self.$right.children(":not(span):not(.hidden)");$options.length&&self.moveToLeft($options,e),$(this).blur()}),self.actions.$undo.on("click",function(e){e.preventDefault(),self.undo(e)}),self.actions.$redo.on("click",function(e){e.preventDefault(),self.redo(e)}),self.actions.$moveUp.on("click",function(e){e.preventDefault(),self.doNotSortRight=!0;var $options=self.$right.find(":selected:not(span):not(.hidden)");$options.first().prev().before($options),$(this).blur()}),self.actions.$moveDown.on("click",function(e){e.preventDefault(),self.doNotSortRight=!0;var $options=self.$right.find(":selected:not(span):not(.hidden)");$options.last().next().after($options),$(this).blur()})},moveToRight:function($options,event,silent,skipStack){var self=this;return"function"==typeof self.callbacks.moveToRight?self.callbacks.moveToRight(self,$options,event,silent,skipStack):"function"!=typeof self.callbacks.beforeMoveToRight||silent||self.callbacks.beforeMoveToRight(self.$left,self.$right,$options)?($options.each(function(index,option){var $option=$(option);if(self.options.ignoreDisabled&&$option.is(":disabled"))return!0;if($option.parent().is("optgroup")){var $leftGroup=$option.parent(),$rightGroup=self.$right.find('optgroup[label="'+$leftGroup.prop("label")+'"]');$rightGroup.length||($rightGroup=$leftGroup.clone(),$rightGroup.children().remove()),$option=$rightGroup.append($option),$leftGroup.removeIfEmpty()}self.$right.move($option)}),skipStack||(self.undoStack.push(["right",$options]),self.redoStack=[]),"function"!=typeof self.callbacks.sort||silent||self.doNotSortRight||self.$right.mSort(self.callbacks.sort),"function"!=typeof self.callbacks.afterMoveToRight||silent||self.callbacks.afterMoveToRight(self.$left,self.$right,$options),self):!1},moveToLeft:function($options,event,silent,skipStack){var self=this;return"function"==typeof self.callbacks.moveToLeft?self.callbacks.moveToLeft(self,$options,event,silent,skipStack):"function"!=typeof self.callbacks.beforeMoveToLeft||silent||self.callbacks.beforeMoveToLeft(self.$left,self.$right,$options)?($options.each(function(index,option){var $option=$(option);if($option.is("optgroup")||$option.parent().is("optgroup")){var $rightGroup=$option.is("optgroup")?$option:$option.parent(),$leftGroup=self.$left.find('optgroup[label="'+$rightGroup.prop("label")+'"]');$leftGroup.length||($leftGroup=$rightGroup.clone(),$leftGroup.children().remove()),$option=$leftGroup.append($option.is("optgroup")?$option.children():$option),$rightGroup.removeIfEmpty()}self.$left.move($option)}),skipStack||(self.undoStack.push(["left",$options]),self.redoStack=[]),"function"!=typeof self.callbacks.sort||silent||self.$left.mSort(self.callbacks.sort),"function"!=typeof self.callbacks.afterMoveToLeft||silent||self.callbacks.afterMoveToLeft(self.$left,self.$right,$options),self):!1},undo:function(event){var self=this,last=self.undoStack.pop();if(last)switch(self.redoStack.push(last),last[0]){case"left":self.moveToRight(last[1],event,!1,!0);break;case"right":self.moveToLeft(last[1],event,!1,!0)}},redo:function(event){var self=this,last=self.redoStack.pop();if(last)switch(self.undoStack.push(last),last[0]){case"left":self.moveToLeft(last[1],event,!1,!0);break;case"right":self.moveToRight(last[1],event,!1,!0)}}},Multiselect}($);$.multiselect={defaults:{startUp:function($left,$right){$right.find("option").each(function(index,option){var $option=$left.find('option[value="'+option.value+'"]'),$parent=$option.parent();$option.remove(),"OPTGROUP"==$parent.prop("tagName")&&$parent.removeIfEmpty()})},beforeMoveToRight:function(){return!0},afterMoveToRight:function(){},beforeMoveToLeft:function(){return!0},afterMoveToLeft:function(){},sort:function(a,b){return"NA"==a.innerHTML?1:"NA"==b.innerHTML?-1:a.innerHTML>b.innerHTML?1:-1}}};var ua=window.navigator.userAgent,isIE=ua.indexOf("MSIE ")+ua.indexOf("Trident/")+ua.indexOf("Edge/")>-3;$.fn.multiselect=function(options){return this.each(function(){var $this=$(this),data=$this.data("crlcu.multiselect"),settings=$.extend({},$.multiselect.defaults,$this.data(),"object"==typeof options&&options);data||$this.data("crlcu.multiselect",data=new Multiselect($this,settings))})},$.fn.move=function($options){return this.append($options).find("option").prop("selected",!1),this},$.fn.removeIfEmpty=function(){return this.children().length||this.remove(),this},$.fn.mShow=function(){return this.removeClass("hidden").show(),isIE&&this.each(function(index,option){$(option).parent().is("span")&&$(option).parent().replaceWith(option),$(option).show()}),this},$.fn.mHide=function(){return this.addClass("hidden").hide(),isIE&&this.each(function(index,option){$(option).parent().is("span")||$(option).wrap("<span>").hide()}),this},$.fn.mSort=function(callback){return this.find("option").sort(callback).appendTo(this),this},$.expr[":"].search=function(elem,index,meta){return $(elem).text().toUpperCase().indexOf(meta[3].toUpperCase())>=0}});
