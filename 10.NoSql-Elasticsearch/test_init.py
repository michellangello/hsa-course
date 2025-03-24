import unittest
from unittest.mock import patch, MagicMock
from init import create_index, populate_index, elastic_client, index_name

class TestElasticsearchFunctions(unittest.TestCase):

    @patch('init.elastic_client')
    def test_create_index(self, mock_elastic_client):
        mock_elastic_client.indices.exists.return_value = False
        mock_elastic_client.indices.create.return_value = {"acknowledged": True}

        create_index()

        mock_elastic_client.indices.exists.assert_called_once_with(index=index_name)
        mock_elastic_client.indices.create.assert_called_once()

    @patch('init.helpers.bulk')
    @patch('builtins.open', create=True)
    def test_populate_index(self, mock_open, mock_bulk):
        mock_open.return_value.__enter__.return_value = ["word1\n", "word2\n"]
        mock_bulk.return_value = (2, [])

        populate_index()

        mock_bulk.assert_called_once()
        mock_open.assert_called_once_with('english_words.txt', 'r')

if __name__ == '__main__':
    unittest.main()