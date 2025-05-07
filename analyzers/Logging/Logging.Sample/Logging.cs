using System;
using Arc4u.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Logging.Sample;

public class Logging
{
	public Logging()
	{
		ILogger<Logging> logger = new LoggerFactory().CreateLogger<Logging>();
		logger.Technical().Debug("asdf");
		logger.Technical().Error("asdf");
		logger.Technical().Exception(new Exception("asdf"));
		logger.Technical().Warning("asdf");
		logger.Technical().Fatal("asdf");
		logger.Technical().Information("asdf");
		logger.Technical().Information("asdf").Log();
		
		logger.Business().Debug("asdf");
		logger.Business().Error("asdf");
		logger.Business().Exception(new Exception("asdf"));
		logger.Business().Warning("asdf");
		logger.Business().Fatal("asdf");
		logger.Business().Information("asdf");
		logger.Business().Information("asdf").Log();
	}
}