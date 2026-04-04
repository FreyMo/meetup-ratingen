#include <stdio.h>
#include <stdlib.h>

typedef int ErrorCode;

#define ERR_OK      0
#define ERR_INVALID 1

typedef struct {
  int value;
} Item;

ErrorCode populate_item(Item* item) {
  item->value = 5;

  return ERR_OK;
}

void do_something() {
  Item item;
  ErrorCode error = populate_item(&item);

  if (error == ERR_OK) {
    // item can be used here
  }
}

Item* create_item() {
  Item* item = malloc(sizeof(Item));

  item->value = 5;

  return item;
}

void create_something() {
  Item* item = create_item();

  if (item != NULL) {
    // item can be used here
  }
}

int main(int argc, char** argv) {
  printf("hello, world");

  do_something();
  create_something();

  return 0;
}
