using ConwayGameOfLife.Application.Abstractions;
using ConwayGameOfLife.Application.Common;
using ConwayGameOfLife.Application.Repositories;

namespace ConwayGameOfLife.Application.CommandAndQueries.Board.Register;

internal sealed class RegisterBoardCommandHandler : ICommandHandler<RegisterBoardCommand, Guid>
{
    private readonly IBoardRepository _boardRepository;

    public RegisterBoardCommandHandler(IBoardRepository boardRepository) =>
        _boardRepository = boardRepository;

    public async Task<ResultObject<Guid>> Handle(RegisterBoardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _boardRepository.RegisterBoard(request.Name, request.State);

            return board.Id;
        }
        catch (Exception ex) 
        {
            return ResultObject.Error<Guid>(ex.Message);
        }
    }
}
