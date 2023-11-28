#include <stdio.h>
#include <string.h>
int convert_char_to(char ch)
{
    switch (ch)
    {
        case 'a':
            return 0;
        case 'b':
            return 1;
        case 'c':
            return 2;
        case 'd':
            return 3;
        case 'e':
            return 4;
        case 'f':
            return 5;
        case 'g':
            return 6;
        case 'h':
            return 7;
    }
}

char* move(char board[72], char *pos)
{
    char tmp;
    tmp=board[convert_char_to(pos[0])+((int)(pos[1])-49)*9];
    board[convert_char_to(pos[0])+((int)(pos[1])-49)*9]='0';
    board[convert_char_to(pos[2])+((int)(pos[3])-49)*9]=tmp;
    return board;   
}

void main()
{
    //printf("hello\n");
    //char board[72]="rnbqkbnr/pppppppp/00000000/00000000/00000000/00000000/PPPPPPPP/RNBQKBNR";
    //strcpy(board, move(board,"a2a4"));
    //printf("%s\n", board);

    char sendInfo[200]="";
    char ch[2];
    char txt[100];
    char lobbyinfo[4];

    

    for (int i = 0; i < 6; i++)
    {
        strncat(sendInfo, "lobby: ", 8);
        sprintf(ch, "%i", i);
        strncat(sendInfo, ch, 1);
        strncat(sendInfo, " contains: ", 12);
        
        strncat(sendInfo, "\n", 2);
    }
    printf("%s",sendInfo);

}

