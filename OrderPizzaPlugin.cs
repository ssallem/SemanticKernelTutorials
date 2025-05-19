using Microsoft.SemanticKernel;
using SK_Tutorials.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK_Tutorials
{
    public class OrderPizzaPlugin 
    {
        private readonly IPizzaService _pizzaService;
        private readonly IUserContext _userContext;
        private readonly IPaymentService _paymentService;

        public OrderPizzaPlugin (IPizzaService pizzaService, IUserContext userContext, IPaymentService paymentService)
        {
            _pizzaService = pizzaService;
            _userContext = userContext;
            _paymentService = paymentService;
        }        

        [KernelFunction("get_pizza_menu")]
        public async Task<Menu> GetPizzaMenuAsync() => await _pizzaService.GetMenuAsync();

        [KernelFunction("add_pizza_to_cart")]
        [Description("Add a pizza to the user's cart; returns the new item and updated cart")]
        public async Task<CartDelta> AddPizzaToCart(PizzaSize size, PizzaToppings toppings, int quantity = 1, string specialInstructions = "")
        {
            Guid cartId = _userContext.GetCartId();
            return await _pizzaService.AddPizzaToCartAsync(cartId, size, toppings, quantity, specialInstructions);
        }

        [KernelFunction("remove_pizza_from_cart")]
        public async Task<RemovePizzaResponse> RemovePizzaFromCart(int pizzaId)
        {
            Guid cartId = _userContext.GetCartId();
            return await _pizzaService.RemovePizzaFromCart(cartId, pizzaId);
        }

        [KernelFunction("get_pizza_from_cart")]
        [Description("Returns the specific details of a pizza in the user's cart")]
        public async Task<Pizza> GetPizzaFromCart(int pizzaId)
        {
            Guid cartId = await _userContext.GetCartIdAsync();
            return await _pizzaService.GetPizzaFromCart(cartId, pizzaId);
        }

        [KernelFunction("get_cart")]
        [Description("Returns the user's current cart")]
        public async Task<Cart> GetCart()
        {
            Guid cartId = await _userContext.GetCartIdAsync();
            return await _pizzaService.GetCart(cartId);
        }

        [KernelFunction("checkout")]
        [Description("Checkouts the user's cart")]
        public async Task<CheckoutResponse> Checkout()
        {
            Guid cartId = await _userContext.GetCartIdAsync();
            Guid paymentId = await _paymentService.RequestPaymentFromUserAsync(cartId);
            return await _pizzaService.Checkout(cartId, paymentId);
        }

    }
}
