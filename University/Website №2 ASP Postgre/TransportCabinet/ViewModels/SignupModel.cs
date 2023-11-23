using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models
{
	public class SignupModel
	{
		[Required (ErrorMessage = "Не указан логин")]
		[Remote(action: "CheckLogin", controller: "Auth", ErrorMessage = "Логин уже используется")]
		public string Login { get; set; }

		[Required (ErrorMessage = "Не указан E-mail")]
		[Remote(action: "CheckEmail", controller: "Auth", ErrorMessage = "E-mail уже используется")]
		public string Email { get; set; }

		[Required (ErrorMessage = "Не указан пароль")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage = "Подтвердите пароль")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		public string ConfirmPassword { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Серия и номер паспорта должны состоять из 10 символов")]
        [Required(ErrorMessage = "Не указан серия и номер паспорта")]
		public string passport_num { get; set; }
	}
}
