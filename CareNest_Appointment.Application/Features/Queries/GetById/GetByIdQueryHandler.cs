using CareNest_Appointment.Application.Features.Queries.GetById;
using CareNest_Appointment.Application.Interfaces.CQRS.Queries;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Domain.Entitites;

namespace CareNest_Appointment.Application.Features.Queries.GetById
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, Appointment>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Appointment> HandleAsync(GetByIdQuery query)
        {
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(query.Id);

            if (appointment == null)
            {
                throw new Exception(MessageConstant.NotFound);
            }
            return appointment;
        }
    }
}
