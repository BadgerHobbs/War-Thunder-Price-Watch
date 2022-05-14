from flask import Flask, render_template, jsonify, request
import json
import sqlite3
from pydantic import BaseModel
import datetime
from typing import List

# Load database into memory
#print('Loading database into memory...')
connection = sqlite3.connect('warthunder_store.db', check_same_thread=False)
#connection = sqlite3.connect(':memory:', check_same_thread=False)
#source.backup(connection)

class PriceHistory(BaseModel):

    id: int
    price: float
    timestamp: datetime.datetime

class StoreItem(BaseModel):

    id: int
    name: str
    url: str
    description: str
    price_history: List[PriceHistory]

def get_store_item_with_full_price_history(item_id):

    cursor = connection.cursor()

    # Get store item information
    cursor.execute("SELECT * FROM StoreItems WHERE id = ?", (item_id,))
    store_item_db = cursor.fetchone()

    store_item = StoreItem(
        id=store_item_db[0],
        name=store_item_db[1],
        url=store_item_db[2],
        description=store_item_db[3],
        price_history=[]
    )

    # Get price history for store item
    cursor.execute("SELECT * FROM PriceHistory WHERE storeitemid = ?", (item_id,))
    price_history_db = cursor.fetchall()

    price_history = []
    for price_history_db_row in price_history_db:
        price_history.append(PriceHistory(
            id=price_history_db_row[0],
            price=price_history_db_row[2],
            timestamp=datetime.datetime.strptime(price_history_db_row[3], '%m/%d/%Y %H:%M:%S')
        ))

    cursor.close()

    store_item.price_history = price_history

    # Return store item with price history
    return store_item

def get_store_items_with_last_two_prices():

    cursor = connection.cursor()

    # Get all store items
    cursor.execute("SELECT * FROM StoreItems")
    store_items_db = cursor.fetchall()

    store_items = []
    for store_item_db_row in store_items_db:
        store_items.append(StoreItem(
            id=store_item_db_row[0],
            name=store_item_db_row[1],
            url=store_item_db_row[2],
            description=store_item_db_row[3],
            price_history=[]
        ))

    # Get latest two price history for all store items
    for store_item in store_items:
        cursor.execute("SELECT * FROM PriceHistory WHERE storeitemid = ? ORDER BY timestamp DESC LIMIT 2", (store_item.id,))
        price_history_db = cursor.fetchall()

        price_history = []
        for price_history_db_row in price_history_db:
            price_history.append(PriceHistory(
                id=price_history_db_row[0],
                price=price_history_db_row[2],
                timestamp=datetime.datetime.strptime(price_history_db_row[3], '%m/%d/%Y %H:%M:%S')
            ))

        store_item.price_history = price_history

    cursor.close()

    # Return store items with last two prices
    return store_items

app = Flask(__name__)
app.debug = True

@app.route("/")
def main():
    return render_template('home.html')

@app.route("/product")
def product():
    return render_template('product.html')

@app.route("/api/store-item")
def api_store_item():
    item_id = int(request.args.get('id'))
    store_item = get_store_item_with_full_price_history(item_id)
    return jsonify(json.loads(store_item.json()))

@app.route("/api/store-items")
def api_store_items():
    store_items = get_store_items_with_last_two_prices()
    return jsonify([json.loads(store_item.json()) for store_item in store_items])

if __name__ == "__main__":
    app.run(host='0.0.0.0', port=5000)