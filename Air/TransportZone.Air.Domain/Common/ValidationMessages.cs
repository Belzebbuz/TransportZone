namespace TransportZone.Air.Domain.Common;

public static class ValidationMessages
{
	public static string Required(string name) => $"Поле {name} является обязательным";
	public static string AlreadyExist(string name, string id) => $"Сущность: {name} c Id: {id} уже добавлена";
	public static string MoreThan(string name, int value) => $"Поле {name} должно быть больше {value}";
	public static string LessThan(string name, int value) => $"Поле {name} должно быть меньше {value}";
}