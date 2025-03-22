using StackExchange.Redis;
using System;

namespace Redis;

public sealed class RedisClient(
		string? connection_string = null,
		bool use_aws_ecs = false,
		TLSOptions? tls_options = null
	) : IDisposable 
{
	private ConnectionMultiplexer? connection_;
	private IDatabase? database_;

	private readonly string? explicit_connection_string_ = connection_string;
	private readonly bool use_aws_ecs_ = use_aws_ecs;
	private readonly TLSOptions ? tls_options_ = tls_options;

	private const string ECS_ENDPOINT_ENV = "";
	private const string ECS_PORT_ENV     = "REDIS_PORT";
    private const string ECS_PASSWORD_ENV = "REDIS_PASSWORD";

	public bool Connect()
	{
		try
		{
			var options = BuildConfigurationOptions();

			connection_ = ConnectionMultiplexer.Connect(options);
			database_ = connection_.GetDatabase();

			return connection_ != null && connection_.IsConnected;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"RedisClient Connect Error: {ex.Message}");
			return false;
		}
	}

	public IDatabase? GetDatabase(int index = -1) 
	{
		if (database_ == null || connection_ == null)
		{
			return null;
		}

		if (index < 0)
		{
			return database_;
		}
		return connection_.GetDatabase(index);
	}

	public bool Set(string key, string value, TimeSpan? expiry = null)
	{
		if (database_ == null)
		{
			return false;
		}

		return database_.StringSet(key, value, expiry);
	}

	public string? Get(string key)
	{
		if (database_ == null)
		{
			return null;
		}

		return database_.StringGet(key);
	}

	public bool Delete(string key)
	{
		if (database_ == null)
		{
			return false;
		}

		return database_.KeyDelete(key);
	}

	public void Dispose()
	{
		if (connection_ != null)
		{
			connection_.Close();
			connection_.Dispose();
			connection_ = null;
		}
	}

	private ConfigurationOptions BuildConfigurationOptions()
	{
		var options = new ConfigurationOptions();

		if (explicit_connection_string_ != null)
		{
			options.EndPoints.Add(explicit_connection_string_);
		}
		else if (use_aws_ecs_)
		{
			var endpoint = Environment.GetEnvironmentVariable(ECS_ENDPOINT_ENV);
			var port = Environment.GetEnvironmentVariable(ECS_PORT_ENV);
			var password = Environment.GetEnvironmentVariable(ECS_PASSWORD_ENV);

			if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(port))
			{
				options.EndPoints.Add($"{endpoint}:{port}");
			}
			if (!string.IsNullOrEmpty(password))
			{
				options.Password = password;
			}
		}

		if (tls_options_ is not null && tls_options_.UseSSL())
		{
			options.Ssl = true;
			options.SslHost = tls_options_.SSLHost();
			options.Password = tls_options_.Password();

			if (!string.IsNullOrEmpty(tls_options_.CertPath()))
			{
				options.CertificateValidation += (_, _, _, _) => true;
			}
		}

		return options;
	}
}
