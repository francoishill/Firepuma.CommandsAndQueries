﻿using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.DatabaseRepositories.Abstractions.Repositories;

namespace Firepuma.CommandsAndQueries.MongoDb.Repositories;

internal interface ICommandExecutionRepository : IRepository<CommandExecutionMongoDbEvent>
{
}