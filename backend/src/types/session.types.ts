import { RowDataPacket } from 'mysql2';

export interface Session{
    user_id:number;
    session_id: number;
    time_started?: string | null;
    time_ended?:string | null;
    languages?: Language[];

}
export interface SessionRow extends RowDataPacket, Session {}

export interface Language {
    id:number;
    name: string;
}

export interface SessionLanguage {
    session_id:number;
    language_id:number;
}
