using AutoMapper;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class AgentController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public AgentController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int? orderId = null)
    {
        if (orderId.HasValue)
        {
            // Fetch the order if an ID is provided
            var order = await _orderService.GetOrderById(orderId.Value);
            var orderVm = _mapper.Map<OrderVM>(order);
            return View(orderVm);
        }

        // No order ID provided or order not found
        return View(new OrderVM());
    }

    [HttpPost]
    public async Task<IActionResult> FindOrder(int orderId)
    {
        // Redirect to the Index action with the orderId parameter
        return RedirectToAction("Index", new { orderId });
    }

    public async Task<IActionResult> Order()
    {
        // This could be used for creating a new order
        var orderVm = new OrderVM();
        return View(orderVm);
    }
}