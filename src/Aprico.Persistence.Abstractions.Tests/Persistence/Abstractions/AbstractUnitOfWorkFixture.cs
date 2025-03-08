#region Copyright & License

// Copyright © 2024 - 2025 Aprico Consultants
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Aprico.Persistence.Abstractions;

public abstract class AbstractUnitOfWorkFixture
{
	#region Nested Type: CommitAsync

	public class CommitAsync : AbstractUnitOfWorkFixture
	{
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[Fact]
		public async Task ThrowsWhenAlreadyCommitted()
		{
			using var unitOfWork = new AbstractUnitOfWorkDummy();
			await unitOfWork.CommitAsync();
			await Invoking(async () => await unitOfWork.CommitAsync())
				.Should()
				.ThrowAsync<InvalidOperationException>()
				.WithMessage("This unit of work has already committed.");
		}

		[Fact]
		public async Task ThrowsWhenCalledAfterDispose()
		{
			var unitOfWork = new AbstractUnitOfWorkDummy();
			unitOfWork.Dispose();
			await Invoking(async () => await unitOfWork.CommitAsync())
				.Should()
				.ThrowAsync<ObjectDisposedException>();
		}
	}

	#endregion

	#region Nested Type: Dispose

	public class Dispose : AbstractUnitOfWorkFixture
	{
		[Fact]
		public void DisposeCanBeCalledMultipleTimes()
		{
			var unitOfWork = new AbstractUnitOfWorkDummy();
			unitOfWork.Dispose();
			Invoking(() => unitOfWork.Dispose())
				.Should()
				.NotThrow();
		}
	}

	#endregion

	#region Nested Type: UnitOfWorkDummy

	private sealed class AbstractUnitOfWorkDummy : AbstractUnitOfWork
	{
		#region Base Class Member Overrides

		protected override Task CommitAsyncCore(CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}

		[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
		public override DbTransaction? DbTransaction { get; }

		public override Task FlushAsync(CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}

		#endregion
	}

	#endregion
}
