using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace eBookStore.Infrastructure.Services;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderNotificationService> _logger;

    public OrderNotificationService(
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<OrderNotificationService> logger)
    {
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrderConfirmationEmailAsync(string email, OrderDto order)
    {
        try
        {
            string subject = $"Order Confirmation #{order.Id}";
            string htmlMessage = GenerateOrderConfirmationEmail(order);

            await _emailService.SendEmailAsync(email, subject, htmlMessage);

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private string GenerateOrderConfirmationEmail(OrderDto order)
    {
        string formattedDate = order.OrderDate.ToString("MMMM dd, yyyy at h:mm tt");

        var emailContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Order Confirmation</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        max-width: 600px;
                        margin: 0 auto;
                    }}
                    .header {{
                        background-color: #4CAF50;
                        color: white;
                        padding: 15px;
                        text-align: center;
                    }}
                    .content {{
                        padding: 20px;
                    }}
                    .order-details {{
                        margin-bottom: 20px;
                    }}
                    .order-items {{
                        width: 100%;
                        border-collapse: collapse;
                        margin-bottom: 20px;
                    }}
                    .order-items th, .order-items td {{
                        border: 1px solid #ddd;
                        padding: 8px;
                        text-align: left;
                    }}
                    .order-items th {{
                        background-color: #f2f2f2;
                    }}
                    .total {{
                        font-weight: bold;
                        text-align: right;
                    }}
                    .footer {{
                        background-color: #f1f1f1;
                        padding: 10px;
                        text-align: center;
                        font-size: 12px;
                    }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Thank You for Your Order!</h1>
                </div>
                <div class='content'>
                    <p>Dear Customer,</p>
                    <p>We're pleased to confirm that we've received your order. Below are the details of your purchase:</p>
                    
                    <div class='order-details'>
                        <h2>Order Information</h2>
                        <p><strong>Order Number:</strong> #{order.Id}</p>
                        <p><strong>Order Date:</strong> {formattedDate}</p>
                        <p><strong>Status:</strong> {order.Status}</p>
                    </div>
                    
                    <h2>Order Summary</h2>
                    <table class='order-items'>
                        <thead>
                            <tr>
                                <th>Item</th>
                                <th>Quantity</th>
                                <th>Unit Price</th>
                                <th>Total</th>
                            </tr>
                        </thead>
                        <tbody>
            ";

        foreach (var item in order.OrderItems)
        {
            decimal itemTotal = item.UnitPrice * item.Quantity;
            emailContent += $@"
                            <tr>
                                <td></td>
                                <td>{item.Quantity}</td>
                                <td>${item.UnitPrice.ToString("F2")}</td>
                                <td>${itemTotal.ToString("F2")}</td>
                            </tr>";
        }

        emailContent += $@"
                        </tbody>
                    </table>
                    
                    <div class='total'>
                        <p>Total Amount: ${order.TotalPrice.ToString("F2")}</p>
                    </div>
                    
                    <p>We're preparing your order for shipment and will notify you once it's on its way.</p>
                    <p>If you have any questions about your order, please contact our customer service at {_configuration["CompanyInfo:SupportEmail"]} or call us at {_configuration["CompanyInfo:SupportPhone"]}.</p>
                    
                    <p>Thank you for shopping with us!</p>
                </div>
                <div class='footer'>
                    <p>&copy; {DateTime.Now.Year} {_configuration["CompanyInfo:Name"]}. All rights reserved.</p>
                    <p>This email was sent to confirm your recent order. Please do not reply to this email.</p>
                </div>
            </body>
            </html>";

        return emailContent;
    }
}