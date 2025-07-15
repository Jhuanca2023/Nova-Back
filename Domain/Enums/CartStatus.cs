namespace Domain.Enums;
public enum CartShopStatus
{
    Active = 1,  //  (en uso)
    Completed = 2,  //  (compra finalizada)
    Canceled = 3,  //  (abandonado)
    Inactive = 4  // Carrito inactivo (limpio/desactivado tras pago)
}