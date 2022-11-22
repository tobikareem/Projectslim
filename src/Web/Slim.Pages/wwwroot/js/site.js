// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function checkIsWholeNumber(n) {
    var result = (n - Math.floor(n)) !== 0;

    if (result)
        return false;
    else
        return true;
} 

function SetCartIconData(data) {
    var isWhole = checkIsWholeNumber(data);
    if (isWhole) {
        $('#totalPrice').text('$' + data);

        // append a small html element to #totalPrice to display the cents
        $('#totalPrice').append('<small id="totalPriceRoundUp">.00</small>');

        $('#cartTotal').text('$' + data);
        $('#cartTotal').append('<small id="totalPriceRoundUp">.00</small>');


    } else {
        var priceRoundUp = data.toString().split('.')[1];
        var priceWholePrice = Math.trunc(data);

        $('#totalPrice').text('$' + priceWholePrice);
        $('#totalPrice').append('<small id="totalPriceRoundUp">.' + priceRoundUp + '</small>');

        $('#cartTotal').text('$' + priceWholePrice);
        $('#cartTotal').append('<small id="totalPriceRoundUp">.' + priceRoundUp + '</small>');
    }
}

function GetTotalCartItem(url) {
    $.ajax({
        type: "GET", // define the type of HTTP verb we want to use (POST for our form)
        url: url,

        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: "application/json; charset=utf-8",
        traditional: true,
        async: false,
        success:
            function (data) {
                
                if (data > 0) {
                    $('#cartTotalItems').text(data);
                }
            },
        error: function (data) {
            console.log(data);
        }

    });
}

function GetTotalCartPrice(url) {
    $.ajax({
        type: "GET", // define the type of HTTP verb we want to use (POST for our form)
        url: url,

        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: "application/json; charset=utf-8",
        traditional: true,
        async: false,
        success:
            function (data) {
                
                if (data > 0) {
                    SetCartIconData(data);
                }
            },
        error: function (data) {
            console.log(data);
        }

    });
}

function OnAddItemToCart(productId, url, totalCartUrl) {
    $.ajax({
        url: url,
        type: 'get',
        data: { id: productId },
        success: function (data) {
            if (data > 0) {
                SetCartIconData(data);
            }
            // hide the add to cart button and display a added to cart button
            $('#add-to-cart-' + productId).prop('disabled', true);

            // change the class of the button to btn btn-secondary
            $('#add-to-cart-' + productId).removeClass('btn-primary');
            $('#add-to-cart-' + productId).addClass('btn-secondary');

            // add html to the button to display added to cart <i class="bi bi-bag-plus-fill fs-sm me-1"></i>
            $('#add-to-cart-' + productId).html('<i class="bi bi-bag-plus-fill fs-sm me-1"></i> Added to cart');

            GetTotalCartItem(totalCartUrl);
        }
    });
}
