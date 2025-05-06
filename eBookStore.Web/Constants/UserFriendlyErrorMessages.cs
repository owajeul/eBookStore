namespace eBookStore.Web.Constants;

public static class UserFriendlyErrorMessages
{
    public const string BookServiceError = "We’re having trouble processing your book request at the moment. Please try again later.";
    public const string ArgumentError = "There seems to be an issue with the information you’ve provided. Please check and try again.";
    public const string OrderError = "We couldn’t process your order at this time. Please try again later.";
    public const string GeneralError = "An unexpected error occurred. Please try again later.";
    public const string BookNotFoundError = "The book you are looking for does not exist.";
    public const string OrderNotFoundError = "The order you are looking for does not exist.";
    public const string BookOutOfStockError = "The book you are trying to purchase is currently out of stock.";

}
