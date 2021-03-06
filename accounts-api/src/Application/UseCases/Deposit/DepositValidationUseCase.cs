// <copyright file="DepositValidationUseCase.cs" company="Ivan Paulovich">
// Copyright © Ivan Paulovich. All rights reserved.
// </copyright>

namespace Application.UseCases.Deposit
{
    using System;
    using System.Threading.Tasks;
    using Domain.ValueObjects;
    using Services;

    /// <inheritdoc />
    public sealed class DepositValidationUseCase : IDepositUseCase
    {
        private readonly IDepositUseCase _useCase;
        private IOutputPort? _outputPort;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DepositValidationUseCase" /> class.
        /// </summary>
        /// <param name="useCase"></param>
        public DepositValidationUseCase(IDepositUseCase useCase) => this._useCase = useCase;

        /// <inheritdoc />
        public void SetOutputPort(IOutputPort outputPort)
        {
            this._outputPort = outputPort;
            this._useCase.SetOutputPort(outputPort);
        }

        /// <inheritdoc />
        public Task Execute(Guid accountId, decimal amount, string currency)
        {
            Notification modelState = new Notification();

            if (accountId == Guid.Empty)
            {
                modelState.Add(nameof(accountId), "AccountId is required.");
            }

            if (currency != Currency.Dollar.Code &&
                currency != Currency.Euro.Code &&
                currency != Currency.BritishPound.Code &&
                currency != Currency.Canadian.Code &&
                currency != Currency.Real.Code &&
                currency != Currency.Krona.Code)
            {
                modelState.Add(nameof(currency), "Currency is required.");
            }

            if (amount <= 0)
            {
                modelState.Add(nameof(amount), "Amount should be positive.");
            }

            if (modelState.IsValid)
            {
                return this._useCase
                    .Execute(accountId, amount, currency);
            }

            this._outputPort?
                .Invalid(modelState);

            return Task.CompletedTask;
        }
    }
}
