
$('#category-tabs li a').click(function () {
    $(this).next('ul').slideToggle('500');
    $(this).find('i').toggleClass('fa-minus fa-plus ')
});
