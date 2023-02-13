// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function checkIsWholeNumber(n) {
  var result = n - Math.floor(n) !== 0;

  if (result) return false;
  else return true;
}

function SetCartIconData(data) {
  var isWhole = checkIsWholeNumber(data);
  if (isWhole) {
    $("#totalPrice").text("$" + data);

    // append a small html element to #totalPrice to display the cents
    $("#totalPrice").append('<small id="totalPriceRoundUp">.00</small>');

    $("#cartTotal").text("$" + data);
    $("#cartTotal").append('<small id="totalPriceRoundUp">.00</small>');
  } else {
    var priceRoundUp = data.toString().split(".")[1];
    var priceWholePrice = Math.trunc(data);

    $("#totalPrice").text("$" + priceWholePrice);
    $("#totalPrice").append(
      '<small id="totalPriceRoundUp">.' + priceRoundUp + "</small>"
    );

    $("#cartTotal").text("$" + priceWholePrice);
    $("#cartTotal").append(
      '<small id="totalPriceRoundUp">.' + priceRoundUp + "</small>"
    );
  }
}

function GetTotalCartItem(url) {
  $.ajax({
    type: "GET", // define the type of HTTP verb we want to use (POST for our form)
    url: url,

    beforeSend: function (xhr) {
      xhr.setRequestHeader(
        "XSRF-TOKEN",
        $('input:hidden[name="__RequestVerificationToken"]').val()
      );
    },
    contentType: "application/json; charset=utf-8",
    traditional: true,
    async: false,
    success: function (data) {
      if (data > 0) {
        $("#cartTotalItems").text(data);
      }
    },
    error: function (data) {
      console.log(data);
    },
  });
}

function GetTotalCartPrice(url) {
  $.ajax({
    type: "GET", // define the type of HTTP verb we want to use (POST for our form)
    url: url,

    beforeSend: function (xhr) {
      xhr.setRequestHeader(
        "XSRF-TOKEN",
        $('input:hidden[name="__RequestVerificationToken"]').val()
      );
    },
    contentType: "application/json; charset=utf-8",
    traditional: true,
    async: false,
    success: function (data) {
      if (data > 0) {
        SetCartIconData(data);
      }
    },
    error: function (data) {
      console.log(data);
    },
  });
}

function OnAddItemToCart(productId, url, totalCartUrl, productType) {

  // get SizeSelection radio button id
    var bagSize = $("input[name='SizeSelectionRadioButton']:checked").val();
    if (bagSize == undefined && productType === 'bags') {
       // show the error modal
        $('#errorModal').modal('show');
        return;
    } else {
        // hide the error modal
        $('#errorModal').modal('hide');
    }

       // hide the add to cart button and display a added to cart button
       $("#add-to-cart-" + productId).prop("disabled", true);

       // change the class of the button to btn btn-secondary
       $("#add-to-cart-" + productId).removeClass("btn-primary");
       $("#add-to-cart-" + productId).addClass("btn-secondary");
 
       // add html to the button to display added to cart <i class="bi bi-bag-plus-fill fs-sm me-1"></i>
       $("#add-to-cart-" + productId).html(
         '<i class="bi bi-bag-plus-fill fs-sm me-1"></i> Added to cart'
       );

  $.ajax({
    url: url,
    type: "get",
      data: { id: productId, bagSize },
    success: function (data) {
      if (data > 0) {
        SetCartIconData(data);
      }

      GetTotalCartItem(totalCartUrl);
    },
    error: function (data) {
         // hide the add to cart button and display a added to cart button
         $("#add-to-cart-" + productId).prop("disabled", false);

         // change the class of the button to btn btn-secondary
         $("#add-to-cart-" + productId).removeClass("btn-secondary");
         $("#add-to-cart-" + productId).addClass("btn-primary");
   
         // add html to the button to display added to cart <i class="bi bi-bag-plus-fill fs-sm me-1"></i>
         $("#add-to-cart-" + productId).html(
           '<i class="bi bi-cart fs-lg me-2"></i> Add to cart'
         );
    }
  });
}

function GetSelectedWebPageDetails(url, pageId) {
  $.ajax({
    type: "GET", // define the type of HTTP verb we want to use (POST for our form)
    url: url,
    data: { id: pageId },
    beforeSend: function (xhr) {
      xhr.setRequestHeader(
        "XSRF-TOKEN",
        $('input:hidden[name="__RequestVerificationToken"]').val()
      );
    },
    contentType: "application/json; charset=utf-8",
    traditional: true,
    async: false,
    success: function (data) {
      debugger;
      if (data != null) {
        $("#pageUrlEdit").val(data.url);
        $("#pageNameEdit").val(data.pageName);
        $("#pageDescEdit").val(data.description);
      }
    },
    error: function (data) {
      console.log(data);
    },
  });
}

function OnSelectBag(selection) {
  if (selection == "all") {
    $("#selectAllBag").addClass("active");
    $("#selectMenBag").removeClass("active");
    $("#selectWomenBag").removeClass("active");

    // get the class with the name of productTags and display the element
    $(".productTags").each(function () {
        $(this).show();
    });

  } else if (selection == "men") {
    $("#selectAllBag").removeClass("active");
    $("#selectMenBag").addClass("active");
    $("#selectWomenBag").removeClass("active");

    // get the class with the name of productTags and 
    // check the name attribute of the element to see if it contains men
    // if it does then display the element
    // if it does not then hide the element
    $(".productTags").each(function () {
        var tag = $(this).attr("name");
        debugger;
      // split the tag by the comma and remove the space
      var tags = tag.split(",").map(function (item) {
        return item.trim();
      });

      // if the tags array contains 'women' then hide the element
        if ( tags.includes('women') ) {
            $(this).hide();
        } else {
            $(this).show();
        }
    });
  } else {
    $("#selectAllBag").removeClass("active");
    $("#selectMenBag").removeClass("active");
    $("#selectWomenBag").addClass("active");

    $(".productTags").each(function () {
        var tag = $(this).attr("name");
        debugger;
      // split the tag by the comma and remove the space
      var tags = tag.split(",").map(function (item) {
        return item.trim();
      });

        // if the tags array contains 'men' then hide the element
        if ( tags.includes('men')) {
            $(this).hide();
        } else {
            $(this).show();
        }
    });
  }
}

function dismissModal(id) {

    // hide the error modal

    $(`#${id}`).modal('hide');

  // $('#errorModal').modal('hide');
}
