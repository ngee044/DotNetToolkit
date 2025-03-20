using System.Diagnostics.Contracts;

namespace Redis;
public sealed class TLSOptions
{
	public bool UseSSL() => use_ssl_;
	public string SSLHost() => ssl_host_ ?? string.Empty;
	public string Password() => password_ ?? string.Empty;
	public string CertPath() => cert_path_ ?? string.Empty;
	private bool use_ssl_;
	private string? ssl_host_;
	private string? password_;
	private string? cert_path_;

	public TLSOptions() {}
	public TLSOptions(
		bool use_ssl,
		string ssl_host,
		string password,
		string cert_path
	)
	{
		use_ssl_ = use_ssl;
		ssl_host_ = ssl_host;
		password_ = password;
		cert_path_ = cert_path;
	}
}