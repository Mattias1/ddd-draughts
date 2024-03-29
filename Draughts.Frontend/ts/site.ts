import * as $ from 'jquery'

export function initShowHideLinks(parentIdentifier: string = ''): void {
    $(parentIdentifier + " .show-hide-link").off('click');

    $(parentIdentifier + " .show-hide-link").on('click', function () {
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

    $('html').on('click', function(evt) {
        if ($(evt.target).closest('.hide-on-click-outside').length == 0) {
            $('.hide-on-click-outside').hide();
        }
    });
}
