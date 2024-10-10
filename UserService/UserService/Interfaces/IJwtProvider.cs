namespace UserService.Interfaces
{
	public interface IJwtProvider
	{
		string CreateToken<T>(T user) where T : class;
		bool VerifyToken(string token);
	}
}
