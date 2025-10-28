import { RowDataPacket } from 'mysql2';

export interface Csession{
    session_id: number;
    user_id:number;
    time_started?: string | null;
    time_ended?:string | null;
    title: string;
    languages?: Language[];

}
export interface CsessionRow extends RowDataPacket, Csession {}

export interface Language {
    id:number;
    name: string;
}

export interface CsessionLanguage {
    session_id:number;
    language_id:number;
}
