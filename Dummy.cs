using SK_Tutorials.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK_Tutorials
{
    public class Dummy
    {
    }

    public class DummyPizzaService : IPizzaService
    {
        public Task<Menu> GetMenuAsync() => Task.FromResult(new Menu());
        public Task<CartDelta> AddPizzaToCartAsync(Guid cartId, PizzaSize size, PizzaToppings toppings, int quantity, string specialInstructions) => Task.FromResult(new CartDelta());
        public Task<RemovePizzaResponse> RemovePizzaFromCart(Guid cartId, int pizzaId) => Task.FromResult(new RemovePizzaResponse());
        public Task<Pizza> GetPizzaFromCart(Guid cartId, int pizzaId) => Task.FromResult(new Pizza());
        public Task<Cart> GetCart(Guid cartId) => Task.FromResult(new Cart());
        public Task<CheckoutResponse> Checkout(Guid cartId, Guid paymentId) => Task.FromResult(new CheckoutResponse());
    }

    public class DummyUserContext : IUserContext
    {
        public Guid GetCartId() => Guid.NewGuid();
        public Task<Guid> GetCartIdAsync() => Task.FromResult(Guid.NewGuid());
    }

    public class DummyPaymentService : IPaymentService
    {
        public Task<Guid> RequestPaymentFromUserAsync(Guid cartId) => Task.FromResult(Guid.NewGuid());
    }
}
