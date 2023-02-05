namespace workapi
{
	using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class User : IdentityUser<long>
	{
		public DateTime CreationTime { get; set; }

		[MaxLength(50)]
		public string? WxUnionid { get; set; }
		[MaxLength(50)]
		public string? Wxopenid { get; set; }
		[MaxLength(50)]
		public string? Appopenid { get; set; }
		[MaxLength(50)]
		public string? Cname { get; set; }

	
    }
}
