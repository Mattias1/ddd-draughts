$(document).ready(function () {
    initializeShowHideLinks();
});

function initializeShowHideLinks(parentIdentifier) {
    if (parentIdentifier == null) {
        parentIdentifier = '';
    }

    $(parentIdentifier + " .show-hide-link").unbind('click');

    $(parentIdentifier + " .show-hide-link").click(function () {
        let slide = $(this).hasClass('slide');
        let id = $(this).data('id');
        let cls = $(this).data('class');
        let defaultId = $(this).data('defaultId');
        let defaultClass = $(this).data('defaultClass');

        let el = $('#' + id);
        if (el.is(':hidden')) {
            if (cls) {
                if (slide) {
                    $('.' + cls).slideUp();
                } else {
                    $('.' + cls).hide();
                }
            }
        } else {
            if (defaultId) {
                if (slide) {
                    $('#' + defaultId).slideDown();
                } else {
                    $('#' + defaultId).show();
                }
            }
        }

        if (defaultClass) {
            if (slide) {
                $('.' + defaultClass).slideDown();
            } else {
                $('.' + defaultClass).show();
            }
            if (el.is(':hidden')) {
                el.parent().find('.' + defaultClass).hide();
            }
        }

        if (slide) {
            el.slideToggle();
        } else {
            el.toggle();
        }

        return false;
    });
}