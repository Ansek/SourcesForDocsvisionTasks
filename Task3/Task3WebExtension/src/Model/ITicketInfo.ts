export interface ITicketInfo
{
    id: number;
    departureAirline: string;
    departureFlightNumber: string;
    returnAirline?: string;
    returnFlightNumber?: string;
    price: number;
    originAirport: string;
    destinationAirport: string;
    departureAt: string;
    returnAt: string;
}