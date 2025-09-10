namespace OrganizerPRO.Application.Common.Models;


public class Result : IResult
{
    protected Result(bool succeeded, IEnumerable<string>? errors)
    {
        Succeeded = succeeded;
        // Convert to a read-only list to ensure immutability.
        Errors = errors?.ToList().AsReadOnly() ?? ((IReadOnlyList<string>)Array.Empty<string>());
    }

    public bool Succeeded { get; init; }
    public IReadOnlyList<string> Errors { get; init; }
    public string ErrorMessage => string.Join(", ", Errors);
    public static Result Success() => new(true, Array.Empty<string>());
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());
    public static Result Failure(params IEnumerable<string> errors) => new(false, errors);
    public static Task<Result> FailureAsync(params IEnumerable<string> errors) => Task.FromResult(Failure(errors));
    public void Match(Action onSuccess, Action<string> onFailure)
    {
        if (Succeeded)
            onSuccess();
        else
            onFailure(ErrorMessage);
    }

    public Task MatchAsync(Func<Task> onSuccess, Func<string, Task> onFailure)
        => Succeeded ? onSuccess() : onFailure(ErrorMessage);
}


public class Result<T> : Result, IResult<T>
{
    public T? Data { get; init; }
    protected Result(bool succeeded, IEnumerable<string>? errors, T? data)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, Array.Empty<string>(), data);
    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));
    public static new Result<T> Failure(params IEnumerable<string> errors) => new(false, errors, default);
    public static new Task<Result<T>> FailureAsync(params IEnumerable<string> errors) => Task.FromResult(Failure(errors));
    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (Succeeded)
            onSuccess(Data!);
        else
            onFailure(ErrorMessage);
    }

    public Task MatchAsync(Func<T, Task> onSuccess, Func<string, Task> onFailure)
        => Succeeded ? onSuccess(Data!) : onFailure(ErrorMessage);

    public Result<TResult> Map<TResult>(Func<T, TResult> map)
        => Succeeded ? Result<TResult>.Success(map(Data!)) : Result<TResult>.Failure(Errors);
    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
        => Succeeded ? Result<TResult>.Success(await map(Data!)) : await Result<TResult>.FailureAsync(Errors);
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind)
        => Succeeded ? bind(Data!) : Result<TResult>.Failure(Errors);
    public Task<Result<TResult>> BindAsync<TResult>(Func<T, Task<Result<TResult>>> bind)
        => Succeeded ? bind(Data!) : Result<TResult>.FailureAsync(Errors);
}