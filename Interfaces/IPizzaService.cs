using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK_Tutorials.Interfaces
{
    public interface IPizzaService
    {
        Task<Menu> GetMenuAsync();
        Task<CartDelta> AddPizzaToCartAsync(Guid cartId, PizzaSize size, PizzaToppings toppings, int quantity, string specialInstructions);
        Task<RemovePizzaResponse> RemovePizzaFromCart(Guid cartId, int pizzaId);
        Task<Pizza> GetPizzaFromCart(Guid cartId, int pizzaId);
        Task<Cart> GetCart(Guid cartId);
        Task<CheckoutResponse> Checkout(Guid cartId, Guid paymentId);
    }

    public record Menu;
    public record CartDelta;
    public record RemovePizzaResponse;
    public record Pizza;
    public record Cart;
    public record CheckoutResponse;

    public enum PizzaSize
    {
        Small,
        Medium,
        Large,
    }

    public enum  PizzaToppings
    {
        Cheese,
        Pepperoni,
        Mushrooms,
    }
}
