import { RowDataPacket } from 'mysql2';

export type SessionStatus = 'running' | 'paused' | 'stopped';

export interface Csession{
    session_id: number;
    user_id:number;
    project_id?: number | null;
    time_started?: string | null;
    time_ended?:string | null;
    title: string;
    status: SessionStatus;
    duration_seconds?: number;
    languages?: Language[];
}

export interface CsessionRow extends RowDataPacket, Csession {}

export interface Language {
    language_id: number;
    name: string;
}

export interface LanguageRow extends RowDataPacket, Language {}

export interface CsessionLanguage {
    session_id:number;
    language_id:number;
}

export interface CreateSessionDTO {
    title: string;
    project_id?: number;
    language_ids: number[];
}

export interface UpdateSessionDTO {
    title?: string;
    project_id?: number;
    language_ids?: number[];
}

export interface SessionWithDetails extends Csession {
    project_name?: string;
}
