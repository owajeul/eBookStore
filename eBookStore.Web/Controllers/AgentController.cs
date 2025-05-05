using AutoMapper;
using eBookStore.Application.Common.Utilily;
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
            var order = await _orderService.GetOrderById(orderId.Value);
            var orderVm = _mapper.Map<OrderVM>(order);
            if(orderVm.PaymentMethod == AppConstant.PaymentMethodCreditCard)
            {
                return View(new OrderVM());
            }
            return View(orderVm);
        }
        return View(new OrderVM());
    }

    [HttpPost]
    public IActionResult FindOrder(int orderId)
    {
        return RedirectToAction("Index", new { orderId });
    }
    [HttpPost]
    public async Task<IActionResult> UpdatePaymentStatus(int orderId, string paymentStatus)
    {
        try
        {
            await _orderService.ChangePaymentStatus(orderId, paymentStatus);
            return Ok();
        }
        catch(Exception)
        {
            return BadRequest("Failed to update payment status");
        }
    }
}